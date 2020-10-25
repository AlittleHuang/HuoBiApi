using System;
using HuoBiApi.Models.Trade;
using Microsoft.AspNetCore.Mvc;

namespace HuoBiApi.Controllers {
    [Controller]
    public class TradeController : ControllerBase {
        private readonly TradeService _tradeService;

        public TradeController(TradeService tradeService) {
            _tradeService = tradeService;
        }

        /// <summary>
        /// 历史成交
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet("/api/history/trade")]
        public ActionResult<TradeData> Get(string symbol, int size = 20) {
            return Ok(_tradeService.GetTradeData(symbol, size));
        }
    }
}