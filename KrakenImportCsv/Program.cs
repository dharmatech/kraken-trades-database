using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using KrakenTradesDatabase;
using EFCore.BulkExtensions;
using CsvHelper.Configuration.Attributes;

namespace KrakenImportCsv
{
    public class CsvRow
    {
        [Index(0)]
        public long TimeStamp { get; set; }

        [Index(1)]
        public decimal Price { get; set; }

        [Index(2)]
        public decimal Quantity { get; set; }
    }

    public class Program
    {
        static string directory = @"C:\Users\dharm\Dropbox\Documents\VisualStudio\KrakenTradesDatabase\KrakenTradesDatabase\kraken-csv-data\Kraken_Trading_History";

        static void initialize_from_csv(string filename)
        {
            var path = Path.Combine(directory, filename);

            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader,
                new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = false
                }))
            using (var db = new AppDbContext())
            {
                var name = filename.Substring(0, filename.Length - 4);

                if (db.Symbols.Any(symbol => symbol.Name == name))
                {
                    Console.WriteLine("Already added {0}", name);
                    return;
                }

                Console.WriteLine("Adding {0}", name);

                db.Symbols.Add(new Symbol() { Name = name });
                db.SaveChanges();

                var symbol_id = db.Symbols.First(symbol => symbol.Name == name).Id;

                var records = csv.GetRecords<CsvRow>().Select(row => new Trade()
                {
                    Price = row.Price,
                    Quantity = row.Quantity,
                    TimeStamp = DateTimeOffset.FromUnixTimeSeconds(row.TimeStamp).UtcDateTime,
                    SymbolId = symbol_id
                });

                var timer_outer = new System.Diagnostics.Stopwatch(); timer_outer.Start();

                while (true)
                {
                    var items = records.Take(100_000).ToList();

                    if (items.Any() == false) break;

                    Console.Write("Starting batch at {0:yyyy-MM-dd}. ", items[0].TimeStamp);

                    var timer = new System.Diagnostics.Stopwatch(); timer.Start();

                    db.BulkInsert(items);
                    db.SaveChanges();

                    timer.Stop();

                    Console.WriteLine("Batch took {0}.", timer.Elapsed);
                }

                timer_outer.Stop();

                Console.WriteLine("{1} trades for symbol took {0}.",
                    timer_outer.Elapsed,
                    db.Trades.Where(trade => trade.SymbolId == symbol_id).Count());

                Console.WriteLine("Records per second: {0:N0}", db.Trades.Where(trade => trade.SymbolId == symbol_id).Count() / timer_outer.Elapsed.TotalSeconds);

                Console.WriteLine();
            }
        }

        static void import_from_directory()
        {
            var directory_info = new DirectoryInfo(directory);

            var file_infos = directory_info.GetFiles("*.csv");

            foreach (var filename in file_infos.Select(info => info.Name))
            {
                initialize_from_csv(filename);
            }
        }

        public static void Main(string[] args)
        {
            SQLitePCL.Batteries.Init();

            import_from_directory();
        }
    }
}