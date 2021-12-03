// See https://aka.ms/new-console-template for more information
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using EFCore.BulkExtensions;
using Kraken.Net;
using Kraken.Net.Objects;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

SQLitePCL.Batteries.Init();

var directory = @"C:\Users\dharm\Dropbox\Documents\VisualStudio\KrakenTradesDatabase\KrakenTradesDatabase\kraken-csv-data\Kraken_Trading_History";

void initialize_from_csv(string filename)
{
    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
    {
        HasHeaderRecord = false
    };
        
    var path = Path.Combine(directory, filename);

    using (var reader = new StreamReader(path))
    using (var csv = new CsvReader(reader, config))
    {
        var name = filename.Substring(0, filename.Length - 4);

        using (var db = new AppContext())
        {
            if (db.Symbols.Any(symbol => symbol.Name == name))
            {
                Console.WriteLine("Already added {0}", name);
                return;
            }

            Console.WriteLine("Adding {0}", name);

            //if (db.Symbols.Any(symbol => symbol.Name == name) == false)
            //{
            //    var symbol = new Symbol() { Name = name };

            //    db.Symbols.Add(symbol);
            //    db.SaveChanges();
            //}

            var symbol = new Symbol() { Name = name };

            db.Symbols.Add(symbol);
            db.SaveChanges();

            var symbol_id = db.Symbols.First(symbol => symbol.Name == name).Id;

            var records = csv.GetRecords<CsvRow>().Select(row => new Trade()
            {
                Price = row.Price,
                Quantity = row.Quantity,
                TimeStamp = DateTimeOffset.FromUnixTimeSeconds(row.TimeStamp).UtcDateTime,
                //Symbol = symbol
                SymbolId = symbol_id
            });

            
            var timer_outer = new System.Diagnostics.Stopwatch();
            timer_outer.Start();
                        
            while (true)
            {
                var items = records.Take(100_000).ToList();

                if (items.Any() == false) break;
                                
                Console.Write("Starting batch at {0:yyyy-MM-dd}. ", items[0].TimeStamp);

                var timer = new System.Diagnostics.Stopwatch();

                timer.Start();

                db.BulkInsert(items);

                //db.Trades.AddRange(items);

                db.SaveChanges();
                //db.BulkSaveChanges();

                timer.Stop();

                Console.WriteLine("Batch took {0}.", timer.Elapsed);
            }

            timer_outer.Stop();

            //Console.WriteLine("Total added: {0}", db.Trades.Where(trade => trade.SymbolId == symbol_id).Count());
                                   
            Console.WriteLine("{1} trades for symbol took {0}.", 
                timer_outer.Elapsed,
                db.Trades.Where(trade => trade.SymbolId == symbol_id).Count());

            Console.WriteLine("Records per second: {0:N0}", db.Trades.Where(trade => trade.SymbolId == symbol_id).Count() / timer_outer.Elapsed.TotalSeconds);

            Console.WriteLine();
        }
    }
}

//initialize_from_csv("WBTCXBT.csv");

//initialize_from_csv("RAYUSD.csv");

//initialize_from_csv("DOTUSD.csv");

void import_from_directory()
{
    //var directory_info = new DirectoryInfo(@"C:\Users\dharm\Dropbox\Documents\VisualStudio\KrakenTradesDatabase\KrakenTradesDatabase\kraken-csv-data\Kraken_Trading_History");

    var directory_info = new DirectoryInfo(directory);

    var file_infos = directory_info.GetFiles("*.csv");

    foreach (var filename in file_infos.Select(info => info.Name))
    {
        initialize_from_csv(filename);
    }
}

import_from_directory();

public class CsvRow
{
    [CsvHelper.Configuration.Attributes.Index(0)]
    //public DateTime TimeStamp { get; set; }
    public long TimeStamp { get; set; }

    [CsvHelper.Configuration.Attributes.Index(1)]
    public decimal Price { get; set; }

    [CsvHelper.Configuration.Attributes.Index(2)]
    public decimal Quantity { get; set; }
}


[Microsoft.EntityFrameworkCore.Index(nameof(Name), IsUnique = true)]
public class Symbol
{
    public int Id { get; set; }
    public string Name { get; set; }
}

[Microsoft.EntityFrameworkCore.Index(nameof(TimeStamp))]
public class Trade
{
    public int Id { get; set; }

    public int SymbolId { get; set; }

    public Symbol Symbol { get; set; } // navigation property
    public decimal Price { get; set; }
    public decimal Quantity { get; set; }
    public DateTime TimeStamp { get; set; }

    public OrderSide Side { get; set; }
    public OrderTypeMinimal Type { get; set; }
        
    public Trade() { }

    public Trade(KrakenTrade trade)
    {
        Price = trade.Price;
        Quantity = trade.Quantity;
        TimeStamp = trade.Timestamp;
        Side = trade.Side;
        Type = trade.Type;
    }
}

public class AppContext : DbContext
{
    public DbSet<Trade> Trades { get; set; }
    public DbSet<Symbol> Symbols { get; set; }

    //public DbSet<KrakenTrade> KrakenTrades { get; set; }

    //public AppContext()
    //{
    //    var folder = Environment.SpecialFolder.LocalApplicationData;

    //    var path = Environment.GetFolderPath(folder);

    //    DbPath

    //}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;

        var path = Environment.GetFolderPath(folder);

        var db_path = $"{path}{System.IO.Path.DirectorySeparatorChar}kraken-trades.db";

        //optionsBuilder.UseSqlite("Data Source=kraken-trades.db");

        optionsBuilder.UseSqlite($"Data Source={db_path}");
    }


}