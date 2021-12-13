using Kraken.Net.Objects;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenTradesDatabase
{
    [Index(nameof(Name), IsUnique = true)]
    public class Symbol
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    [Index(nameof(TimeStamp))]
    [Index(nameof(Price))]
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

        //public Trade() { }

        //public Trade(KrakenTrade trade)
        //{
        //    Price = trade.Price;
        //    Quantity = trade.Quantity;
        //    TimeStamp = trade.Timestamp;
        //    Side = trade.Side;
        //    Type = trade.Type;
        //}
    }

    public enum CandleUnit { Year, Month, Week, Day, Hour, Minute }

    //public class CandleInterval
    //{
    //    public int Id { get; set; }

    //}

    public class Candle
    {
        public int Id { get; set; }

        public int SymbolId { get; set; }
        public Symbol Symbol { get; set; } // navigation property

        public DateTimeOffset DateTimeOffset { get; set; }

        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Open { get; set; }
        public decimal Close { get; set; }

        public CandleUnit CandleUnit { get; set; }
        public int Interval { get; set; }

    }

    public class AppDbContext : DbContext
    {
        public DbSet<Trade> Trades { get; set; }
        public DbSet<Symbol> Symbols { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //var folder = Environment.SpecialFolder.LocalApplicationData;

            //var path = Environment.GetFolderPath(folder);

            //var db_path = $"{path}{System.IO.Path.DirectorySeparatorChar}kraken-trades.db";

            //optionsBuilder.UseSqlite($"Data Source={db_path}");

            var password =
                File.ReadAllText(
                    Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "postgres-password"));
               
            optionsBuilder.UseNpgsql($"Host=localhost;Database=kraken-trades;Username=postgres;Password={password}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresEnum<OrderSide>();
            modelBuilder.HasPostgresEnum<OrderTypeMinimal>();
        }

        static AppDbContext()
        {
            NpgsqlConnection.GlobalTypeMapper.MapEnum<OrderSide>();
            NpgsqlConnection.GlobalTypeMapper.MapEnum<OrderTypeMinimal>();
        }
    }
}
