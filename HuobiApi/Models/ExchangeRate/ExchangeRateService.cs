using System.Net.Http;
using System.Timers;
using HuoBiApi.Utils;

namespace HuoBiApi.Models.ExchangeRate
{
    public class ExchangeRateService
    {
        private readonly HttpClient _httpClient;

        private double _exchangeRate;
        
        public double ExchangeRate => _exchangeRate;


        public ExchangeRateService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            UpdateData();
            SetTimer();
        }
        
        private void SetTimer()
        {
            var timer = new Timer(1000 * 60 * 60 * 4);
            timer.Elapsed += (a, b) =>
            {
                UpdateData();
            };
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private void UpdateData()
        {
            var result = _httpClient.GetStringAsync("https://api.xcurrency.com/rate/mid/latest?apiKey=adc642b8-082f-4615-8c25-fc842f31384a&symbols=USD&quote=CNY").Result;
            var deserialize = Json.Deserialize<Result>(result);
            _exchangeRate = deserialize.Rates.USD;
        }

        struct Result
        {
            public bool Success { get; set; }
            public Rate Rates { get; set; }
        }

        struct Rate
        {
            public double USD { get; set; }
        }

    }
}