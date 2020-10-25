namespace HuoBiApi.Models.Trade {
    public struct TradeTick {
        public long Id { get; set; }
        public long Ts { get; set; }
        public TradeData[] Data { get; set; }
    }

    public struct TradeData {
        public long Ts { get; set; }

        // public string Id { get; set; }
        public long TradeId { get; set; }
        public double Amount { get; set; }
        public double Price { get; set; }
        public string Direction { get; set; }
    }
}