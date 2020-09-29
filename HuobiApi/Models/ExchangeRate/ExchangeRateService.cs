using System;
using System.Net.Http;
using System.Timers;
using HuoBiApi.Utils;

namespace HuoBiApi.Models.ExchangeRate
{
    public class ExchangeRateService
    {
        private readonly HttpClient _httpClient;

        public double ExchangeRate { get; private set; }


        public ExchangeRateService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            UpdateData();
            SetTimer();
        }

        private void SetTimer()
        {
            var timer = new Timer(1000 * 60 * 60 * 4);
            timer.Elapsed += (a, b) => { UpdateData(); };
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private void UpdateData()
        {
            var url = "https://api.xcurrency.com/rate/mid/latest?apiKey=adc642b8-082f-4615-8c25-fc842f31384a";
            var result = _httpClient.GetStringAsync(url).Result;
            var deserialize = Json.Deserialize<Result>(result);
            var exchangeRate = 1 / deserialize.Rates.CNY;
            ExchangeRate = Math.Round(exchangeRate, 4);
        }

        struct Result
        {
            public bool Success { get; set; }
            public Rate Rates { get; set; }
        }

        struct Rate
        {
            public double CNY { get; set; }
        }
    }
}