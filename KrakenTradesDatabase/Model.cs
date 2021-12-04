﻿using Kraken.Net.Objects;
using Microsoft.EntityFrameworkCore;
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

    public class AppDbContext : DbContext
    {
        public DbSet<Trade> Trades { get; set; }
        public DbSet<Symbol> Symbols { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;

            var path = Environment.GetFolderPath(folder);

            var db_path = $"{path}{System.IO.Path.DirectorySeparatorChar}kraken-trades.db";

            optionsBuilder.UseSqlite($"Data Source={db_path}");
        }
    }
}
