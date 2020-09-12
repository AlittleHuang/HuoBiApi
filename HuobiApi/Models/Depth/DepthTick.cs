namespace HuoBiApi.Models.Depth
{
    public struct DepthTick
    {
        public long Version { get; set; }
        public long Ts { get; set; }
        public double[][] Bids { get; set; }
        public double[][] Asks { get; set; }
    }
}