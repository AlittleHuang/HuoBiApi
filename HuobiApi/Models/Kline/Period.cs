namespace HuoBiApi.Models.Kline {
    public enum Period {
        Min1,
        Min5,
        Min15,
        Min30,
        Min60,
        Hour4,
        Day1,
        Mon1,
        Week1,
        Year1
    }


    public static class PeriodExtensions {
        private static readonly string[] Keys = {
            "1min", "5min", "15min", "30min", "60min", "4hour", "1day", "1mon", "1week", "1year"
        };

        public static string GetId(this Period period) {
            return Keys[(int) period];
        }
    }
}