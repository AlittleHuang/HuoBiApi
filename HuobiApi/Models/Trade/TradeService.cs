using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using HuobiApi.Models.Trade;
using HuobiApi.Utils;
using HuoBiApi.Utils;
using Test;
using WebSocketSharp;

namespace HuoBiApi.Models.Trade
{
    public class TradeService
    {
        private readonly Dictionary<string, ConcurrentQueue<TradeData>> _cache =
            new Dictionary<string, ConcurrentQueue<TradeData>>();

        private readonly HttpClient _httpClient;
        private readonly SymbolsService _symbolsService;
        private WebSocket _webSocket;

        public TradeService(SymbolsService symbolsService, HttpClient httpClient)
        {
            _symbolsService = symbolsService;
            _httpClient = httpClient;
            Init();
            SetTimer();
        }

        public List<TradeData> GetTradeData(string symbol, int size = 20)
        {
            if (!_symbolsService.Exist(symbol)) throw new ArgumentException($"symbol {symbol} error");

            var key = $"market.{symbol}.trade.detail";
            if (!_cache.ContainsKey(key))
            {
                lock (this)
                {
                    if (!_cache.ContainsKey(key))
                    {
                        var history = _httpClient
                            .GetStringAsync($"https://api.huobi.pro/market/history/trade?symbol={symbol}&size=100")
                            .Result;
                        history = history.Replace("trade-id", "tradeId");
                        var tradeHistory = Json.Deserialize<TradeHistory>(history);
                        _cache[key] = new ConcurrentQueue<TradeData>();
                        var tradeTicks = new LinkedList<TradeData>();
                        foreach (var item in tradeHistory.Data.SelectMany(tick => tick.Data))
                        {
                            tradeTicks.AddFirst(item);
                        }

                        foreach (var data in tradeTicks)
                        {
                            _cache[key].Enqueue(data);
                        }

                        var sendData = $"{{\"sub\":\"{key}\",\"id\":\"{Guid.NewGuid()}\"}}";
                        _webSocket.Send(sendData);
                    }
                }
            }

            for (var i = 0; i < 50; i++)
            {
                if (_cache.ContainsKey(key)) break;

                Thread.Sleep(200);
            }

            var result = new List<TradeData>(_cache[key]);
            result.Reverse();
            return result.GetRange(0, Math.Min(result.Count, size));
        }


        private void Init()
        {
            var webSocket = _webSocket = HuobiWebSocketClient.GetWebSocket();
            webSocket.OnMessage += (sender, e) =>
            {
                var data = GZipDecompresser.Decompress(e.RawData);
                if (data.Contains("ping"))
                    (sender as WebSocket)?.Send(data.Replace("ping", "pong"));
                else
                    try
                    {
                        var updateEvent = Json.Deserialize<TradeTickUpdateEvent>(data);
                        if (!_cache.ContainsKey(updateEvent.Ch))
                        {
                            _cache[updateEvent.Ch] = new ConcurrentQueue<TradeData>();
                        }

                        foreach (var tradeData in updateEvent.Tick.Data)
                        {
                            _cache[updateEvent.Ch].Enqueue(tradeData);
                        }

                        while (_cache[updateEvent.Ch].Count > 100)
                        {
                            _cache[updateEvent.Ch].TryDequeue(out _);
                        }
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }
            };
            webSocket.OnClose += (sender, e) => _cache.Clear();
            webSocket.OnError += (sender, e) => WebSocketUtils.CloseWebSocket(webSocket);
            webSocket.Connect();
        }

        private void SetTimer()
        {
            var timer = new System.Timers.Timer(5000);
            timer.Elapsed += (a, b) =>
            {
                if (_webSocket.IsAlive) return;
                WebSocketUtils.CloseWebSocket(_webSocket);
                _cache.Clear();
                Init();
            };
            timer.AutoReset = true;
            timer.Enabled = true;
        }
    }
}