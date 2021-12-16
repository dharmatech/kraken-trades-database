using KrakenTradesDatabase;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace KrakenCandles // Note: actual namespace depends on the project name.
{
    public class Program
    {
        static void UpdateCandles(string name, CandleUnit unit, int interval)
        {
            using (var db = new AppDbContext())
            {
                var timer = new System.Diagnostics.Stopwatch(); 
                
                timer.Start();

                var symbol_id = db.Symbols.First(symbol => symbol.Name == name).Id;

                Console.Write("Querying database for existing candles. ");

                var candles = db.Candles
                    .Where(candle => candle.SymbolId == symbol_id)
                    .Where(candle => candle.CandleUnit == unit)
                    .Where(candle => candle.Interval == interval)
                    .OrderBy(candle => candle.DateTimeOffset);
                                
                var start = candles.Any() ? candles.Last().DateTimeOffset : DateTime.UnixEpoch;

                if (candles.Any())
                    db.Candles.Remove(candles.Last());

                Console.WriteLine("Done.");

                double grouping_value(Trade trade)
                {
                    if (unit == CandleUnit.Month)
                        return (trade.TimeStamp.Year * 12 + trade.TimeStamp.Month) / ((double)interval);

                    else if (unit == CandleUnit.Week)
                        return trade.TimeStamp.Year * 100 +
                            new GregorianCalendar().GetWeekOfYear(trade.TimeStamp, CalendarWeekRule.FirstDay, DayOfWeek.Monday)
                            / (double)interval;

                    else if (unit == CandleUnit.Day)
                        return new DateTimeOffset(trade.TimeStamp).ToUnixTimeSeconds() / TimeSpan.FromDays(interval).TotalSeconds;

                    else if (unit == CandleUnit.Hour)
                        return new DateTimeOffset(trade.TimeStamp).ToUnixTimeSeconds() / TimeSpan.FromHours(interval).TotalSeconds;

                    //else if (unit == CandleUnit.Hour)
                    //    return (trade.TimeStamp.Year * 100000 + trade.TimeStamp.DayOfYear * 100 + trade.TimeStamp.Hour) / 
                    //        (double) interval;

                    else if (unit == CandleUnit.Minute)
                        return new DateTimeOffset(trade.TimeStamp).ToUnixTimeSeconds() / TimeSpan.FromMinutes(interval).TotalSeconds;

                    else
                        throw new Exception("Unsupported CandleUnit value.");
                }

                Func<Trade, long> grouping_function()
                {
                    return trade =>
                        (long)
                        Math.Round(
                            grouping_value(trade),
                            0,
                            MidpointRounding.AwayFromZero);
                }
                                                              
                var result = db.Trades
                    .Where(trade => trade.TimeStamp >= start)
                    .Where(trade => trade.SymbolId == symbol_id).ToList()
                    .GroupBy(grouping_function())
                    .Select(group =>
                        new Candle()
                        {
                            SymbolId = symbol_id,

                            //DateTimeOffset = group.Min(trade => trade.TimeStamp).ToUniversalTime().Date,

                            DateTimeOffset = group.Min(trade => trade.TimeStamp).ToUniversalTime(),

                            High = group.Max(trade => trade.Price),
                            Low = group.Min(trade => trade.Price),

                            Open = group.OrderBy(trade => trade.TimeStamp).First().Price,
                            Close = group.OrderBy(trade => trade.TimeStamp).Last().Price,

                            CandleUnit = unit,
                            Interval = interval
                        });

                db.Candles.AddRange(result);

                db.SaveChanges();

                foreach (var elt in result)
                {
                    Console.WriteLine("{0:yyyy-MM-dd HH mm} {1,10:F} {2,10:F} {3,10:F} {4,10:F}",
                        elt.DateTimeOffset,
                        elt.Open,
                        elt.High,
                        elt.Low,
                        elt.Close);
                }
                                        
                Console.WriteLine();
                Console.WriteLine(timer.Elapsed);
            }
        }

        public static void Main(string[] args)
        {
            UpdateCandles("XBTUSD", CandleUnit.Month, 1);
            //UpdateCandles("XBTUSD", CandleUnit.Month, 2);
            //UpdateCandles("XBTUSD", CandleUnit.Month, 3);
            //UpdateCandles("XBTUSD", CandleUnit.Month, 15);

            UpdateCandles("XBTUSD", CandleUnit.Week, 1);
            //UpdateCandles("XBTUSD", CandleUnit.Week, 2);

            UpdateCandles("XBTUSD", CandleUnit.Day, 1);
            //UpdateCandles("XBTUSD", CandleUnit.Day, 2);
            //UpdateCandles("XBTUSD", CandleUnit.Day, 10);

            UpdateCandles("XBTUSD", CandleUnit.Hour, 1);
            //UpdateCandles("XBTUSD", CandleUnit.Hour, 2);
            //UpdateCandles("XBTUSD", CandleUnit.Hour, 3);

            UpdateCandles("XBTUSD", CandleUnit.Minute, 15);

            //using (var db = new AppDbContext())
            //{
            //    var timer = new System.Diagnostics.Stopwatch(); timer.Start();

            //    var symbol_id = db.Symbols.First(symbol => symbol.Name == "XBTUSD").Id;

            //    var candles = db.Candles
            //        .Where(candle => candle.SymbolId == symbol_id)
            //        .Where(candle => candle.CandleUnit == CandleUnit.Month)
            //        .Where(candle => candle.Interval == 1)
            //        .OrderBy(candle => candle.DateTimeOffset);

            //    DateTimeOffset start = DateTime.UnixEpoch;

            //    if (candles.Any())
            //    {
            //        start = candles.Last().DateTimeOffset;

            //        db.Candles.Remove(candles.Last());
            //    }

            //    // 1 month

            //    var result =
            //        db.Trades

            //            .Where(trade => trade.TimeStamp >= start)

            //            .Where(trade => trade.SymbolId == symbol_id).ToList()

            //            .GroupBy(trade =>
            //                (long)
            //                Math.Round(
            //                    (trade.TimeStamp.Year * 12 + trade.TimeStamp.Month) / 1.0,
            //                    0,
            //                    MidpointRounding.AwayFromZero))

            //            .Select(group =>
            //                new Candle()
            //                {
            //                    SymbolId = symbol_id,

            //                    DateTimeOffset = group.Min(trade => trade.TimeStamp).ToUniversalTime().Date,

            //                    High = group.Max(trade => trade.Price),
            //                    Low = group.Min(trade => trade.Price),

            //                    CandleUnit = CandleUnit.Month,
            //                    Interval = 1
            //                });

            //    db.Candles.AddRange(result);

            //    db.SaveChanges();

            //    // 9 month

            //    //var result =
            //    //    db.Trades.Where(trade => trade.SymbolId == symbol_id).ToList()

            //    //        .GroupBy(trade =>
            //    //            (long)
            //    //            Math.Round(
            //    //                (trade.TimeStamp.Year * 12 + trade.TimeStamp.Month) / 9.0,
            //    //                0,
            //    //                MidpointRounding.AwayFromZero))

            //    //        .Select(group =>
            //    //            new Candle()
            //    //            {
            //    //                DateTime = group.Min(trade => trade.TimeStamp),

            //    //                High = group.Max(trade => trade.Price),
            //    //                Low = group.Min(trade => trade.Price)
            //    //            });

            //    // 15 months

            //    //var result =
            //    //    db.Trades.Where(trade => trade.SymbolId == symbol_id).ToList()

            //    //        .GroupBy(trade =>
            //    //            (long)
            //    //            Math.Round(
            //    //                (trade.TimeStamp.Year * 12 + trade.TimeStamp.Month) / 15.0,
            //    //                0,
            //    //                MidpointRounding.AwayFromZero))

            //    //        .Select(group =>
            //    //            new Candle()
            //    //            {
            //    //                DateTime = group.Min(trade => trade.TimeStamp),

            //    //                High = group.Max(trade => trade.Price),
            //    //                Low = group.Min(trade => trade.Price)
            //    //            });


            //    // 1 week

            //    //var result =
            //    //    db.Trades.Where(trade => trade.SymbolId == symbol_id).ToList()

            //    //        .GroupBy(trade =>
            //    //            (long)
            //    //            Math.Round(

            //    //                trade.TimeStamp.Year * 52 +

            //    //                new GregorianCalendar().GetWeekOfYear(
            //    //                    trade.TimeStamp,
            //    //                    CalendarWeekRule.FirstDay,
            //    //                    DayOfWeek.Monday)

            //    //                / 1.0,

            //    //                0,
            //    //                MidpointRounding.AwayFromZero))

            //    //        .Select(group =>
            //    //            new Candle()
            //    //            {
            //    //                DateTime = group.Min(trade => trade.TimeStamp),

            //    //                High = group.Max(trade => trade.Price),
            //    //                Low = group.Min(trade => trade.Price)
            //    //            });

            //    // 1 week

            //    //var result =
            //    //    db.Trades.Where(trade => trade.SymbolId == symbol_id).ToList()
            //    //        .GroupBy(trade =>
            //    //            (long)
            //    //            Math.Round(
            //    //                trade.TimeStamp.Year * 100 +
            //    //                new GregorianCalendar().GetWeekOfYear(trade.TimeStamp, CalendarWeekRule.FirstDay, DayOfWeek.Monday)
            //    //                / 1.0,
            //    //                0,
            //    //                MidpointRounding.AwayFromZero))

            //    //        .Select(group =>
            //    //            new Candle()
            //    //            {
            //    //                DateTime = group.Min(trade => trade.TimeStamp),

            //    //                High = group.Max(trade => trade.Price),
            //    //                Low = group.Min(trade => trade.Price)
            //    //            });

            //    // 30 days

            //    //var result =
            //    //    db.Trades.Where(trade => trade.SymbolId == symbol_id).ToList()

            //    //        .GroupBy(trade =>
            //    //            (int)
            //    //            Math.Round(
            //    //                new DateTimeOffset(trade.TimeStamp).ToUnixTimeSeconds() / TimeSpan.FromDays(30).TotalSeconds,
            //    //                0,
            //    //                MidpointRounding.AwayFromZero))

            //    //        .Select(group =>
            //    //            new Candle()
            //    //            {
            //    //                DateTime = group.Min(trade => trade.TimeStamp),

            //    //                High = group.Max(trade => trade.Price),
            //    //                Low = group.Min(trade => trade.Price)
            //    //            });

            //    // 3 day

            //    //var result =
            //    //    db.Trades.Where(trade => trade.SymbolId == symbol_id).ToList()

            //    //        .GroupBy(trade => 
            //    //            (int)
            //    //            Math.Round(
            //    //                new DateTimeOffset(trade.TimeStamp).ToUnixTimeSeconds() / TimeSpan.FromDays(3).TotalSeconds,
            //    //                0,
            //    //                MidpointRounding.AwayFromZero))

            //    //        .Select(group =>
            //    //            new Candle()
            //    //            {
            //    //                DateTime = group.Min(trade => trade.TimeStamp),

            //    //                High = group.Max(trade => trade.Price),
            //    //                Low = group.Min(trade => trade.Price)
            //    //            });



            //    foreach (var elt in result)
            //    {
            //        Console.WriteLine("{0:yyyy-MM-dd} {1,10:F} {2,10:F} {3,10:F} {4,10:F}",
            //            elt.DateTimeOffset,
            //            elt.Open,
            //            elt.High,
            //            elt.Low,
            //            elt.Close
            //            );
            //    }

            //    timer.Stop();

            //    Console.WriteLine();
            //    Console.WriteLine(timer.Elapsed);
            //}
        }
    }
}