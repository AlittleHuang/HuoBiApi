namespace HuoBiApi.Models.Trade
{
    public struct TradeTickUpdateEvent
    {
        public string Ch { get; set; }
        public long Ts { get; set; }
        public TradeTick Tick { get; set; }
    }
}