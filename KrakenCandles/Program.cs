using KrakenTradesDatabase;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KrakenCandles
{
    public class Candle
    {
        public DateTime DateTime { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Open { get; set; }
        public decimal Close { get; set; }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            using (var db = new AppDbContext())
            {
                var timer = new System.Diagnostics.Stopwatch(); timer.Start();
                
                var symbol_id = db.Symbols.First(symbol => symbol.Name == "XBTUSD").Id;

                var result =
                    db.Trades
                        .Where(trade => trade.SymbolId == symbol_id)
                        .GroupBy(trade => new { Year = trade.TimeStamp.Year, Month = trade.TimeStamp.Month })
                        .Select(group =>
                            new Candle()
                            {
                                DateTime = new DateTime(group.Key.Year, group.Key.Month, 1),
                                High = group.Max(trade => trade.Price),
                                Low = group.Min(trade => trade.Price)
                            });

                // 00:00:11
                // Fast! But does not have the Open and Close DateTime values.

                foreach (var elt in result)
                {
                    Console.WriteLine("{0:yyyy-MM} {1,10:F} {2,10:F} {3,10:F} {4,10:F}",
                        elt.DateTime,
                        elt.Open,
                        elt.High,
                        elt.Low,
                        elt.Close
                        );
                }
                                
                timer.Stop();

                Console.WriteLine();
                Console.WriteLine(timer.Elapsed);
            }
        }
    }
}