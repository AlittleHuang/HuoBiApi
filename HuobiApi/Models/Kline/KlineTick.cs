namespace HuoBiApi.Models.Kline {
    public struct KlineTick {
        public long Id { get; set; }
        public double Amount { get; set; }
        public int Count { get; set; }
        public double Open { get; set; }
        public double Close { get; set; }
        public double Low { get; set; }
        public double High { get; set; }
        public double Vol { get; set; }
    }
}