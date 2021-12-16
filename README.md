
Project to retrieve the complete market data from the Kraken exchange.

Data is stored in PostgreSQL.

Program is written in C# using Entity Framework Core.

For each trading pair, the `Trades` table stores the following:

- TimeStamp (time of the trade)
- Price (price of the product at the time of trade)
- Quantity (amount purchased)

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

At this point you may run `KrakenImportCsv`. This will import the trading data from the CSV files into the database. This process takes around 5 hours on my system.

The CSV files contain data up to the most recent complete quarter. To get the rest of the data up to the present time, the API must be used.

- Edit [KrakenImportApi/Program.cs](https://github.com/dharmatech/kraken-trades-database/blob/009-postgresql/KrakenImportApi/Program.cs)
  - Specify the trading pair that you'd like to retrieve data for:
  - `import_from_api("XBTUSD");`
  - I recommend that you only retrieve trading pairs that you're interested in since it does hammer there servers pretty hard to grab them all.
