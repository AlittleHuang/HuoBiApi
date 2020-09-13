using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using HuobiApi.Models.Trade;
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
        }

        public List<TradeData> GetTradeData(string symbol)
        {
            if (!_symbolsService.Exist(symbol)) throw null;

            if (!_webSocket.IsAlive) Init();

            var key = $"market.{symbol}.trade.detail";
            if (!_cache.ContainsKey(key))
            {
                lock (this)
                {
                    if (!_cache.ContainsKey(key))
                    {
                        string history = _httpClient.GetStringAsync($"https://api.huobi.pro/market/history/trade?symbol={symbol}&size=100").Result;
                        history = history.Replace("trade-id", "tradeId");
                        TradeHistory tradeHistory = Json.Deserialize<TradeHistory>(history);
                        _cache[key] = new ConcurrentQueue<TradeData>();
                        foreach (var tick in tradeHistory.Data)
                        {
                            foreach (var item in tick.Data)
                            {
                                _cache[key].Enqueue(item);
                            }
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
            return result;
        }


        private void Init()
        {
            _webSocket = HuobiWebSocketClient.GetWebSocket();
            _webSocket.OnMessage += (sender, e) =>
            {
                var data = GZipDecompresser.Decompress(e.RawData);
                if (data.Contains("ping"))
                    _webSocket.Send(data.Replace("ping", "pong"));
                else
                    try
                    {
                        var updateEvent = Json.Deserialize<TradeTickUpdateEvent>(data);
                        if (!_cache.ContainsKey(updateEvent.Ch))
                        {
                            _cache[updateEvent.Ch] = new ConcurrentQueue<TradeData>();
                        }

                        foreach (TradeData tradeData in updateEvent.Tick.Data)
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
            _webSocket.OnClose += (sender, e) =>
            {
                _cache.Clear();
                Init();
            };
            try
            {
                _webSocket.Connect();
            }
            catch(Exception e)
            {
                _cache.Clear();
                throw e;
            }
        }
    }
}