using System;
using HuoBiApi.Models.Kline;
using Microsoft.AspNetCore.Mvc;

namespace HuoBiApi.Controllers {
    [Controller]
    public class KlineController : ControllerBase {
        private readonly KlineService _klineService;

        public KlineController(KlineService klineService) {
            _klineService = klineService;
        }

        /// <summary>
        /// k线
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="period"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet("/api/history/kline")]
        public ActionResult<KlineTick> KlineHistory(string symbol, string period, int size = 200) {
            Period? p = null;

            foreach (Period value in Enum.GetValues(typeof(Period)))
                if (value.GetId() == period)
                    p = value;

            if (p == null) return NotFound("period error");

            var ticks = _klineService.GetTicks(symbol, (Period) p, size);

            return Ok(ticks);
        }
    }
}