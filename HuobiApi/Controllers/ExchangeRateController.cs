using HuoBiApi.Models.ExchangeRate;
using Microsoft.AspNetCore.Mvc;

namespace HuoBiApi.Controllers
{
    [Controller]
    public class ExchangeRateController : ControllerBase
    {
        private readonly ExchangeRateService _exchangeRateService;

        public ExchangeRateController(ExchangeRateService exchangeRateService)
        {
            _exchangeRateService = exchangeRateService;
        }

        [HttpGet("/api/rate/usd-cny")]
        public object Home()
        {
            var exchangeRate = _exchangeRateService.ExchangeRate;
            return Ok(new { success = true, data = exchangeRate });
        }
    }
}