using KrakenTradesDatabase;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KrakenCandles // Note: actual namespace depends on the project name.
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

                //var result =
                //    db.Trades
                //        .Where(trade => trade.SymbolId == symbol_id)
                //        .GroupBy(trade => new { Year = trade.TimeStamp.Year, Month = trade.TimeStamp.Month })
                //        .Select(group =>
                //            new Candle()
                //            {
                //                DateTime = new DateTime(group.Key.Year, group.Key.Month, 1),
                //                High = group.Max(trade => trade.Price),
                //                Low = group.Min(trade => trade.Price)
                //            });

                // 00:00:11
                // Fast! But does not have the Open and Close DateTime values.



                var result =
                    db.Trades
                        .Where(trade => trade.SymbolId == symbol_id)
                        .GroupBy(trade => new { Year = trade.TimeStamp.Year, Month = trade.TimeStamp.Month })
                        .Select(group =>
                            new Candle()
                            {
                                DateTime = new DateTime(group.Key.Year, group.Key.Month, 1),
                                High = group.Max(trade => trade.Price),
                                Low = group.Min(trade => trade.Price),
                                Open = group.OrderBy(trade => trade.TimeStamp).First().Price,
                                Close = group.OrderBy(trade => trade.TimeStamp).Last().Price
                            });

                // Unhandled exception.
                // Npgsql.PostgresException (0x80004005): 42803:
                // subquery uses ungrouped column "t.TimeStamp" from outer query

                // Has Open and Close, however does not compile.


                //var result =
                //    db.Trades
                //        .Where(trade => trade.SymbolId == symbol_id).ToList()
                //        .GroupBy(trade => new { Year = trade.TimeStamp.Year, Month = trade.TimeStamp.Month })
                //        .Select(group =>
                //            new Candle()
                //            {
                //                DateTime = new DateTime(group.Key.Year, group.Key.Month, 1),
                //                High = group.Max(trade => trade.Price),
                //                Low = group.Min(trade => trade.Price),
                //                Open = group.OrderBy(trade => trade.TimeStamp).First().Price,
                //                Close = group.OrderBy(trade => trade.TimeStamp).Last().Price
                //            });

                // Explicit call to ToList()
                // 6 minutes


                //var result =
                //    db.Trades
                //        .Where(trade => trade.SymbolId == symbol_id).ToList()
                //        .GroupBy(trade => new { Year = trade.TimeStamp.Year, Month = trade.TimeStamp.Month })
                //        .Select(group =>
                //            new Candle()
                //            {
                //                DateTime = new DateTime(group.Key.Year, group.Key.Month, 1),
                //                High = group.Max(trade => trade.Price),
                //                Low = group.Min(trade => trade.Price)
                //            });

                // Explicit call to ToList()
                // 00:06:08





                //var result =
                //    db.Trades
                //        .Where(trade => trade.SymbolId == symbol_id)
                //        .GroupBy(trade => new { Year = trade.TimeStamp.Year, Month = trade.TimeStamp.Month })
                //        .Select(group =>
                //            new
                //            {
                //                DateTime = new DateTime(group.Key.Year, group.Key.Month, 1),
                //                High = group.Max(trade => trade.Price),
                //                Low = group.Min(trade => trade.Price)
                //            });

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

                //foreach (var elt in result)
                //{
                //    Console.WriteLine("{0:yyyy-MM} {1,10:F} {2,10:F} {3} {4}",
                //        elt.DateTime,
                //        elt.High,
                //        elt.Low,
                //        elt.open_time,
                //        elt.close_time);
                //}

                timer.Stop();

                Console.WriteLine();
                Console.WriteLine(timer.Elapsed);
            }
        }
    }
}