using HuobiApi.Models.Trade;
using HuoBiApi.Utils;
using System;
using System.IO;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            StreamReader streamReader = File.OpenText("D:\\Users\\huanghaha\\Downloads\\trade.json");
            string text = streamReader.ReadToEnd().Replace("trade-id", "tradeId");
            Console.WriteLine(text);
            TradeHistory tradeHistory = Json.Deserialize<TradeHistory>(text);
            string json = Json.Serialize(tradeHistory);
            Console.WriteLine(json);
            Console.ReadKey();
        }
    }
}
