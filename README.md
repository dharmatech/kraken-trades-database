
Project to retrieve the complete market data from the Kraken exchange.

Data is stored in PostgreSQL.

Program is written in C# using Entity Framework Core.

# Setup instructions

- Download the `Single ZIP File` from the following page on the Kraken site:
  - [Downloadable historical market data (time and sales)](https://support.kraken.com/hc/en-us/articles/360047543791-Downloadable-historical-market-data-time-and-sales)
  - Note: as of 2021-12, this file is over 4.2 GB.
- Edit [Model.cs](https://github.com/dharmatech/kraken-trades-database/blob/009-postgresql/KrakenTradesDatabase/Model.cs)
  - Specify your PostgreSQL username and password:
  - `optionsBuilder.UseNpgsql($"Host=localhost;Database=kraken-trades;Username=postgres;Password={password}");`
- Edit [KrakenImportCsv/Program.cs](https://github.com/dharmatech/kraken-trades-database/blob/009-postgresql/KrakenImportCsv/Program.cs)
  - Change the `directory` to the location of the CSV files you downloaded:
  - `static string directory = @"C:\Users\dharmatech\Kraken_Trading_History";`
