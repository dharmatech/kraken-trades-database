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

                // 1 month

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

                // 2 months

                //var result =
                //    db.Trades                        
                //        .Where(trade => trade.SymbolId == symbol_id)                        
                //        .ToList()
                //        .GroupBy(trade => new
                //        {
                //            Val = (int) Math.Round(
                //                (trade.TimeStamp.Year * 100.0 + trade.TimeStamp.Month) / 2.0,
                //                0,
                //                MidpointRounding.AwayFromZero)
                //        })
                //        .Select(group =>
                //            new Candle()
                //            {
                //                DateTime = group.Min(trade => trade.TimeStamp),

                //                High = group.Max(trade => trade.Price),
                //                Low = group.Min(trade => trade.Price)
                //            });

                // 3 months

                //var result =
                //    db.Trades
                //        .Where(trade => trade.SymbolId == symbol_id)
                //        .ToList()
                //        .GroupBy(trade => new
                //        {
                //            Val = (int)Math.Round(
                //                (trade.TimeStamp.Year * 100.0 + trade.TimeStamp.Month) / 3.0,
                //                0,
                //                MidpointRounding.AwayFromZero)
                //        })
                //        .Select(group =>
                //            new Candle()
                //            {
                //                DateTime = group.Min(trade => trade.TimeStamp),

                //                High = group.Max(trade => trade.Price),
                //                Low = group.Min(trade => trade.Price)
                //            });

                // 15 months

                //var result =
                //    db.Trades
                //        .Where(trade => trade.SymbolId == symbol_id)
                //        .ToList()
                //        .GroupBy(trade => new
                //        {
                //            Val = (int)Math.Round(
                //                (trade.TimeStamp.Year * 100.0 + trade.TimeStamp.Month) / 15.0,
                //                0,
                //                MidpointRounding.AwayFromZero)
                //        })
                //        .Select(group =>
                //            new Candle()
                //            {
                //                DateTime = group.Min(trade => trade.TimeStamp),

                //                High = group.Max(trade => trade.Price),
                //                Low = group.Min(trade => trade.Price)
                //            });

                // 3 day

                //var result =
                //    db.Trades.Where(trade => trade.SymbolId == symbol_id).ToList()

                //        .GroupBy(trade => new
                //        {
                //            Val = (int)Math.Round(
                //                (trade.TimeStamp.Year * 10000.0 + trade.TimeStamp.Month * 100 + trade.TimeStamp.Day) / 3.0,
                //                0,
                //                MidpointRounding.AwayFromZero)
                //        })

                //        .Select(group =>
                //            new Candle()
                //            {
                //                DateTime = group.Min(trade => trade.TimeStamp),

                //                High = group.Max(trade => trade.Price),
                //                Low = group.Min(trade => trade.Price)
                //            });


 
                // 9 month

                var result =
                    db.Trades.Where(trade => trade.SymbolId == symbol_id).ToList()

                        //.GroupBy(trade =>
                        //    (int)
                        //    Math.Round(
                        //        new DateTimeOffset(trade.TimeStamp).ToUnixTimeSeconds() / TimeSpan.FromDays(30).TotalSeconds,
                        //        0,
                        //        MidpointRounding.AwayFromZero))

                        .GroupBy(trade =>
                            (long)
                            Math.Round(
                                (trade.TimeStamp.Year * 12 + trade.TimeStamp.Month) / 9.0,
                                0,
                                MidpointRounding.AwayFromZero))

                        .Select(group =>
                            new Candle()
                            {
                                DateTime = group.Min(trade => trade.TimeStamp),

                                High = group.Max(trade => trade.Price),
                                Low = group.Min(trade => trade.Price)
                            });


                // 30 days

                //var result =
                //    db.Trades.Where(trade => trade.SymbolId == symbol_id).ToList()

                //        .GroupBy(trade =>
                //            (int)
                //            Math.Round(
                //                new DateTimeOffset(trade.TimeStamp).ToUnixTimeSeconds() / TimeSpan.FromDays(30).TotalSeconds,
                //                0,
                //                MidpointRounding.AwayFromZero))

                //        .Select(group =>
                //            new Candle()
                //            {
                //                DateTime = group.Min(trade => trade.TimeStamp),

                //                High = group.Max(trade => trade.Price),
                //                Low = group.Min(trade => trade.Price)
                //            });

                // 3 day

                //var result =
                //    db.Trades.Where(trade => trade.SymbolId == symbol_id).ToList()

                //        .GroupBy(trade => 
                //            (int)
                //            Math.Round(
                //                new DateTimeOffset(trade.TimeStamp).ToUnixTimeSeconds() / TimeSpan.FromDays(3).TotalSeconds,
                //                0,
                //                MidpointRounding.AwayFromZero))

                //        .Select(group =>
                //            new Candle()
                //            {
                //                DateTime = group.Min(trade => trade.TimeStamp),

                //                High = group.Max(trade => trade.Price),
                //                Low = group.Min(trade => trade.Price)
                //            });



                foreach (var elt in result)
                {
                    Console.WriteLine("{0:yyyy-MM-dd} {1,10:F} {2,10:F} {3,10:F} {4,10:F}",
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