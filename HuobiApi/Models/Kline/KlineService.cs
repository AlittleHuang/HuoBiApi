using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HuobiApi.Utils;
using HuoBiApi.Utils;
using Test;
using WebSocketSharp;

namespace HuoBiApi.Models.Kline
{
    public class KlineService
    {
        private readonly HttpClient _httpClient;
        private readonly SymbolsService _symbolsService;

        private const int Size = 2000;

        private readonly Dictionary<string, SortedDictionary<long, KlineTick>> _cache =
            new Dictionary<string, SortedDictionary<long, KlineTick>>();

        private WebSocket _webSocket;

        public KlineService(HttpClient httpClient, SymbolsService symbolsService)
        {
            _httpClient = httpClient;
            _symbolsService = symbolsService;
            Init();
            SetTimer();
        }

        public List<KlineTick> GetTicks(string symbol, Period period, int size = 200)
        {
            if (!_symbolsService.Exist(symbol)) return null;

            var key = GetKey(symbol, period);
            if (!_cache.ContainsKey(key))
            {
                lock (this)
                {
                    if (!_cache.ContainsKey(key))
                    {
                        var sendData = $"{{\"sub\":\"{key}\",\"id\":\"{Guid.NewGuid()}\"}}";
                        _webSocket.Send(sendData);

                        _cache[key] = SortedDictionaryFactory.NewSortedDictionary();
                        _cache[key] = DoGetTicks(symbol, period).Result;
                    }
                }
            }

            var ticks = _cache[key].Values;
            var result = new List<KlineTick>();
            foreach (var tick in ticks)
            {
                if (result.Count < size)
                {
                    result.Add(tick);
                }
                else
                {
                    break;
                }
            }

            return result;
        }

        private static string GetKey(string symbol, Period period)
        {
            return $"market.{symbol}.kline.{period.GetId()}";
        }

        private void Init()
        {
            var webSocket = _webSocket = HuobiWebSocketClient.GetWebSocket();
            webSocket.OnMessage += (sender, e) =>
            {
                var data = GZipDecompresser.Decompress(e.RawData);
                if (data.Contains("ping"))
                    webSocket.Send(data.Replace("ping", "pong"));
                else
                    try
                    {
                        var updateEvent = Json.Deserialize<TrickUpdateEvent>(data);
                        if (!_cache.ContainsKey(updateEvent.Ch))
                            lock (this)
                            {
                                if (!_cache.ContainsKey(updateEvent.Ch))
                                    _cache[updateEvent.Ch] = SortedDictionaryFactory.NewSortedDictionary();
                            }

                        var ticks = _cache[updateEvent.Ch];
                        ticks[updateEvent.Tick.Id] = updateEvent.Tick;
                        while (ticks.Count > Size)
                        {
                            ticks.Remove(ticks.Last().Key);
                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
            };
            webSocket.OnClose += (sender, e) => _cache.Clear();
            webSocket.OnError += (sender, e) => WebSocketUtils.CloseWebSocket(webSocket);
            webSocket.Connect();
        }


        private async Task<SortedDictionary<long, KlineTick>> DoGetTicks(string symbol, Period period)
        {
            var url = $"https://api.huobi.pro/market/history/kline?period={period.GetId()}&symbol={symbol}&size={Size}";
            var kline = await _httpClient.GetStringAsync(url);
            var klineHistory = Json.Deserialize<KlineHistory>(kline);
            var result = SortedDictionaryFactory.NewSortedDictionary();
            foreach (var tick in klineHistory.Data)
                if (!result.ContainsKey(tick.Id))
                    result[tick.Id] = tick;

            return result;
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


    internal struct KlineHistory
    {
        public string Ch { get; set; }
        public string Status { get; set; }
        public long Ts { get; set; }
        public KlineTick[] Data { get; set; }
    }

    internal struct SymbolsResult
    {
        public string Status { get; set; }
        public Dictionary<string, object>[] Data { set; get; }
    }

    internal class SortedDictionaryFactory : IComparer<long>
    {
        private static readonly SortedDictionaryFactory Instance = new SortedDictionaryFactory();

        private SortedDictionaryFactory()
        {
        }

        public int Compare(long x, long y)
        {
            return -x.CompareTo(y);
        }

        public static SortedDictionary<long, KlineTick> NewSortedDictionary()
        {
            return new SortedDictionary<long, KlineTick>(Instance);
        }
    }
}