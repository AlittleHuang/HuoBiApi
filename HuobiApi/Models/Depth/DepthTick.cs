namespace HuoBiApi.Models.Depth {
    public struct DepthTick {
        public long Version { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public long Ts { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double[][] Bids { get; set; }

        public double[][] Asks { get; set; }
    }
}