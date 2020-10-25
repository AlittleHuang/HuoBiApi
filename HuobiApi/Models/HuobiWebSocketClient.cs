using WebSocketSharp;

namespace HuoBiApi.Models {
    public static class HuobiWebSocketClient {
        public static WebSocket GetWebSocket() {
            return new WebSocket("wss://api.huobi.com/ws");
        }
    }
}