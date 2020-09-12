using System;
using System.Collections.Concurrent;
using System.Threading;
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
            _webSocket = HuobiWebSocketClient.GetWebSocket();
            _webSocket.OnMessage += (sender, e) =>
            {
                var data = GZipDecompresser.Decompress(e.RawData);
                if (data.Contains("ping"))
                    _webSocket.Send(data.Replace("ping", "pong"));
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
            _webSocket.OnClose += (sender, e) =>
            {
                _cache.Clear();
                Init();
            };
            _webSocket.Connect();
        }
    }
}