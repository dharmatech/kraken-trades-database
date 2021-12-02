// See https://aka.ms/new-console-template for more information
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Kraken.Net;
using Kraken.Net.Objects;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var client = new KrakenClient();

//var trades = client.GetTradeHistoryAsync("XBTUSD").Result;

//var trades = client.GetTradeHistoryAsync("XBTUSD", DateTime.MinValue).Result;

//var trades = client.GetTradeHistoryAsync("XBTUSD", new DateTime(2000, 1, 1)).Result;

//void initialize()
//{
//    using (var db = new AppContext())
//    {
//        db.AddRange(trades.Data.Data.Select(kraken_trade => new Trade(kraken_trade)));

//        db.SaveChanges();
//    }
//}

//initialize();


void initialize_from_csv()
{
    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
    {
        HasHeaderRecord = false
    };

    using (var reader = new StreamReader(@"C:\Users\dharm\Dropbox\Documents\VisualStudio\KrakenTradesDatabase\KrakenTradesDatabase\kraken-csv-data\Kraken_Trading_History\XBTUSD.csv"))
    using (var csv = new CsvReader(reader, config))
    {
        var records = csv.GetRecords<CsvRow>().Select(row => new Trade() 
        {
            Price = row.Price,
            Quantity = row.Quantity,
            TimeStamp = DateTimeOffset.FromUnixTimeSeconds(row.TimeStamp).UtcDateTime
        });

        using (var db = new AppContext())
        {
            //db.AddRange(records);
            //db.SaveChanges();

            Console.WriteLine(DateTime.Now);

            //foreach (var record in records)
            //{
            //    //Console.WriteLine("{0:yyyy-MM-dd}", record.TimeStamp);
            //    db.Add(record);
            //    db.SaveChanges();
            //}

            //while (true)
            //{
            //    db.AddRange(records.Take(10000));
            //    db.SaveChanges();
            //}

            //while (true)
            //{

            //    //var items = records.Take(10_000).ToList();

            //    var items = records.Take(100_000).ToList();

            //    if (items.Any() == false) break;
                                
            //    db.AddRange(items);
            //    db.SaveChanges();
            //}

            while (true)
            {

                //var items = records.Take(10_000).ToList();

                var items = records.Take(100_000).ToList();

                if (items.Any() == false) break;
                                
                Console.WriteLine("{0:yyyy-MM-dd}", items[0].TimeStamp);

                db.AddRange(items);
                db.SaveChanges();
            }

            Console.WriteLine(DateTime.Now);
        }
    }
}


initialize_from_csv();


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


public class Symbol
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class Trade
{
    public int Id { get; set; }
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