using System;
using WebSocketSharp;

namespace HuobiApi.Utils {
    public static class WebSocketUtils {
        public static void CloseWebSocket(WebSocket webSocket) {
            try {
                webSocket?.Close();
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }
    }
}