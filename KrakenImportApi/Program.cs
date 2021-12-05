using Kraken.Net;
using Kraken.Net.Objects;
using KrakenTradesDatabase;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KrakenImportApi
{
    public class Program
    {
        static Trade FromKrakenTrade(KrakenTrade trade, int symbol_id)
        {
            return new Trade() 
            {
                Price = trade.Price,
                Quantity = trade.Quantity,
                TimeStamp = trade.Timestamp,
                Side = trade.Side,
                Type = trade.Type,
                SymbolId = symbol_id
            };
        }

        static void import_from_api(string name)
        {
            var client = new KrakenClient();

            var timer = new System.Diagnostics.Stopwatch(); timer.Start();

            var count = 0;

            using (var db = new AppDbContext())
            {
                Console.WriteLine("Retrieving most recent trade from database for {0}.", name);
                
                var trade = db.Trades
                    .OrderBy(trade => trade.TimeStamp)
                    .Last(trade => trade.Symbol.Name == name);

                var time_stamp = trade.TimeStamp;

                while (true)
                {
                    Console.Write("Requesting trades via API since {0:yyyy-MM-dd hh:mm:ss}. ", time_stamp);

                    var kraken_trades = client.GetTradeHistoryAsync(name, time_stamp).Result;
                                        
                    if (kraken_trades.Success == false)
                    {
                        if (kraken_trades.Error != null)
                        {
                            Console.WriteLine("{0}. Retrying.", kraken_trades.Error.Message);
                        }
                        else
                        {
                            Console.WriteLine("Issue with API call. Retrying.");
                        }
                        Thread.Sleep(TimeSpan.FromSeconds(5));
                        continue;
                    }

                    var trades = kraken_trades.Data.Data.Select(kraken_trade => 
                        FromKrakenTrade(kraken_trade, trade.SymbolId));

                    Console.WriteLine("Adding trades to database.");
                    db.Trades.AddRange(trades);

                    db.SaveChanges();   

                    count += trades.Count();

                    if (trades.Count() < 1000) break;
                    
                    time_stamp = kraken_trades.Data.Last;

                    Thread.Sleep(TimeSpan.FromSeconds(2));
                }
            }

            timer.Stop();

            Console.WriteLine("Adding {1} trades took {0}.", 
                timer.Elapsed,
                count);
        }

        public static void Main(string[] args)
        {
            //import_from_api("ADAETH");
            //import_from_api("1INCHUSD");
            //import_from_api("AAVEUSD");
            import_from_api("AAVEETH");
        }
    }
}