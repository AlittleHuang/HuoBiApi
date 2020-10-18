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

        /// <summary>
        /// USD 兑换人民币汇率
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/rate/usd-cny")]
        public ActionResult<ExchangeRate> Home()
        {
            var exchangeRate = _exchangeRateService.ExchangeRate;
            return Ok(new ExchangeRate { Success = true, Data = exchangeRate });
        }
    }

    public class ExchangeRate
    {
        /// <summary>
        /// 
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 汇率
        /// </summary>
        public double Data { get; set; }

    }
}