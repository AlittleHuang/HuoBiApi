using HuoBiApi.Models.Trade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HuobiApi.Models.Trade
{
    public class TradeHistory
    {
        public string Ch { get; set; }
        public string Status { get; set; }
        public long Ts { get; set; }
        public List<TradeTick> Data { get; set; }
    }
}
