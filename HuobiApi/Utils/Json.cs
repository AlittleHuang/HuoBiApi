using System.Text.Json;

namespace HuoBiApi.Utils {
    public static class Json {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true,
        };

        public static TValue Deserialize<TValue>(string json, JsonSerializerOptions options = null) =>
            JsonSerializer.Deserialize<TValue>(json, options ?? Options);

        public static string Serialize<TValue>(TValue value, JsonSerializerOptions options = null) =>
            JsonSerializer.Serialize(value, options ?? Options);
    }
}