// See https://aka.ms/new-console-template for more information
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

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
            Console.WriteLine(DateTime.Now);
                        
            while (true)
            {
                //var items = records.Take(10_000).ToList();

                //Console.WriteLine("reading batch of records from CSV");

                var items = records.Take(100_000).ToList();

                if (items.Any() == false) break;
                
                Console.Write("Starting batch at {0:yyyy-MM-dd}. ", items[0].TimeStamp);

                var timer = new System.Diagnostics.Stopwatch();

                timer.Start();

                db.AddRange(items);
                db.SaveChanges();

                timer.Stop();

                Console.WriteLine("Batch took {0}.", timer.Elapsed);

                //db.ChangeTracker.Clear();
            }

            Console.WriteLine(DateTime.Now);
        }
    }
}

initialize_from_csv();

public class CsvRow
{
    [CsvHelper.Configuration.Attributes.Index(0)]
    public long TimeStamp { get; set; }

    [CsvHelper.Configuration.Attributes.Index(1)]
    public decimal Price { get; set; }

    [CsvHelper.Configuration.Attributes.Index(2)]
    public decimal Quantity { get; set; }
}

[Index(nameof(TimeStamp))]
public class Trade
{
    public int Id { get; set; }
    public decimal Price { get; set; }
    public decimal Quantity { get; set; }
    public DateTime TimeStamp { get; set; }
}

public class AppContext : DbContext
{
    public DbSet<Trade> Trades { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;

        var path = Environment.GetFolderPath(folder);

        var db_path = $"{path}{System.IO.Path.DirectorySeparatorChar}kraken-trades.db";
                
        optionsBuilder.UseSqlite($"Data Source={db_path}");
    }
}