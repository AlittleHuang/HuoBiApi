namespace HuoBiApi.Models.Kline
{
    public struct TrickUpdateEvent
    {

        public string Ch { get; set; }
        public long Ts { get; set; }
        public KlineTick Tick { get; set; }

    }
}