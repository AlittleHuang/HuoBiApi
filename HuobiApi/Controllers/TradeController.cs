using System;
using HuoBiApi.Models.Trade;
using Microsoft.AspNetCore.Mvc;

namespace HuoBiApi.Controllers
{
    [Controller]
    public class TradeController : ControllerBase
    {

        private readonly TradeService _tradeService;

        public TradeController(TradeService tradeService)
        {
            _tradeService = tradeService;
        }

        [HttpGet("/api/trade")]
        public object Get(string symbol)
        {
            return _tradeService.GetTradeData(symbol);
        } 
    }
}