using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using HuobiApi.AutomaticInjection.Attributes;
using HuoBiApi.Models.Kline;
using HuoBiApi.Utils;

namespace HuoBiApi.Models {
    [Component]
    public class SymbolsService {
        private readonly HashSet<string> _symbols;

        public SymbolsService(HttpClient httpClient) {
            var result = httpClient
                .GetStringAsync("https://api.huobi.pro/v1/common/symbols").Result;
            var symbolsResult = Json.Deserialize<SymbolsResult>(result);
            lock (this) {
                _symbols = (
                    from dictionary in symbolsResult.Data
                    where dictionary["state"].ToString() == "online"
                    select dictionary["symbol"].ToString()
                ).ToHashSet();
            }
        }

        public bool Exist(string symbol) {
            return _symbols.Contains(symbol);
        }
    }
}