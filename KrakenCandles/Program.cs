﻿using KrakenTradesDatabase;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace KrakenCandles // Note: actual namespace depends on the project name.
{
    //public class Candle
    //{
    //    public DateTime DateTime { get; set; }
    //    public decimal High { get; set; }
    //    public decimal Low { get; set; }
    //    public decimal Open { get; set; }
    //    public decimal Close { get; set; }
    //}

    public class Program
    {

        public static void Main(string[] args)
        {
            using (var db = new AppDbContext())
            {
                var timer = new System.Diagnostics.Stopwatch(); timer.Start();
                                                
                var symbol_id = db.Symbols.First(symbol => symbol.Name == "XBTUSD").Id;

                var candles = db.Candles
                    .Where(candle => candle.SymbolId == symbol_id)
                    .Where(candle => candle.CandleUnit == CandleUnit.Month)
                    .Where(candle => candle.Interval == 1);

                DateTimeOffset start = DateTime.UnixEpoch;

                if (candles.Any())
                {
                    start = candles.Last().DateTimeOffset;

                    db.Candles.Remove(candles.Last());
                }

                // 1 month

                var result =
                    db.Trades
                        
                        .Where(trade => trade.TimeStamp >= start)
                    
                        .Where(trade => trade.SymbolId == symbol_id).ToList()

                        .GroupBy(trade =>
                            (long)
                            Math.Round(
                                (trade.TimeStamp.Year * 12 + trade.TimeStamp.Month) / 1.0,
                                0,
                                MidpointRounding.AwayFromZero))

                        .Select(group =>
                            new Candle()
                            {
                                SymbolId = symbol_id,

                                DateTimeOffset = group.Min(trade => trade.TimeStamp).ToUniversalTime().Date,

                                High = group.Max(trade => trade.Price),
                                Low = group.Min(trade => trade.Price),

                                CandleUnit = CandleUnit.Month,
                                Interval = 1
                            });

                db.Candles.AddRange(result);

                db.SaveChanges();

                // 9 month

                //var result =
                //    db.Trades.Where(trade => trade.SymbolId == symbol_id).ToList()

                //        .GroupBy(trade =>
                //            (long)
                //            Math.Round(
                //                (trade.TimeStamp.Year * 12 + trade.TimeStamp.Month) / 9.0,
                //                0,
                //                MidpointRounding.AwayFromZero))

                //        .Select(group =>
                //            new Candle()
                //            {
                //                DateTime = group.Min(trade => trade.TimeStamp),

                //                High = group.Max(trade => trade.Price),
                //                Low = group.Min(trade => trade.Price)
                //            });

                // 15 months

                //var result =
                //    db.Trades.Where(trade => trade.SymbolId == symbol_id).ToList()

                //        .GroupBy(trade =>
                //            (long)
                //            Math.Round(
                //                (trade.TimeStamp.Year * 12 + trade.TimeStamp.Month) / 15.0,
                //                0,
                //                MidpointRounding.AwayFromZero))

                //        .Select(group =>
                //            new Candle()
                //            {
                //                DateTime = group.Min(trade => trade.TimeStamp),

                //                High = group.Max(trade => trade.Price),
                //                Low = group.Min(trade => trade.Price)
                //            });


                // 1 week

                //var result =
                //    db.Trades.Where(trade => trade.SymbolId == symbol_id).ToList()

                //        .GroupBy(trade =>
                //            (long)
                //            Math.Round(

                //                trade.TimeStamp.Year * 52 +

                //                new GregorianCalendar().GetWeekOfYear(
                //                    trade.TimeStamp,
                //                    CalendarWeekRule.FirstDay,
                //                    DayOfWeek.Monday)

                //                / 1.0,

                //                0,
                //                MidpointRounding.AwayFromZero))

                //        .Select(group =>
                //            new Candle()
                //            {
                //                DateTime = group.Min(trade => trade.TimeStamp),

                //                High = group.Max(trade => trade.Price),
                //                Low = group.Min(trade => trade.Price)
                //            });

                // 1 week

                //var result =
                //    db.Trades.Where(trade => trade.SymbolId == symbol_id).ToList()
                //        .GroupBy(trade =>
                //            (long)
                //            Math.Round(
                //                trade.TimeStamp.Year * 100 +
                //                new GregorianCalendar().GetWeekOfYear(trade.TimeStamp, CalendarWeekRule.FirstDay, DayOfWeek.Monday)
                //                / 1.0,
                //                0,
                //                MidpointRounding.AwayFromZero))

                //        .Select(group =>
                //            new Candle()
                //            {
                //                DateTime = group.Min(trade => trade.TimeStamp),

                //                High = group.Max(trade => trade.Price),
                //                Low = group.Min(trade => trade.Price)
                //            });

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
                        elt.DateTimeOffset,
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