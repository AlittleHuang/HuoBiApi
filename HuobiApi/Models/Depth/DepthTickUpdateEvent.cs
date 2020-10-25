namespace HuoBiApi.Models.Depth {
    public struct DepthTickUpdateEvent {
        public string Ch { get; set; }
        public long Ts { get; set; }

        public DepthTick Tick { get; set; }
    }
}