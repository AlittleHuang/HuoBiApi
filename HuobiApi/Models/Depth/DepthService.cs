using System;
using System.Collections.Concurrent;
using System.Threading;
using HuobiApi.Utils;
using HuoBiApi.Utils;
using Test;
using WebSocketSharp;

namespace HuoBiApi.Models.Depth
{
    public class DepthService
    {
        private readonly ConcurrentDictionary<string, DepthTick> _cache = new ConcurrentDictionary<string, DepthTick>();
        private readonly SymbolsService _symbolsService;
        private WebSocket _webSocket;

        public DepthService(SymbolsService symbolsService)
        {
            _symbolsService = symbolsService;
            Init();
            SetTimer();
        }

        public DepthTick GetDepth(string symbol)
        {
            if (!_symbolsService.Exist(symbol))
            {
                throw null;
            }

            if (!_webSocket.IsAlive)
            {
                Init();
            }

            var key = $"market.{symbol}.depth.step0";
            if (!_cache.ContainsKey(key))
            {
                var sendData = $"{{\"sub\":\"{key}\",\"id\":\"{Guid.NewGuid()}\"}}";
                _webSocket.Send(sendData);
            }

            for (var i = 0; i < 10; i++)
            {
                if (_cache.ContainsKey(key)) break;

                Thread.Sleep(200);
            }

            return _cache[key];
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
                        var updateEvent = Json.Deserialize<DepthTickUpdateEvent>(data);
                        _cache[updateEvent.Ch] = updateEvent.Tick;
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