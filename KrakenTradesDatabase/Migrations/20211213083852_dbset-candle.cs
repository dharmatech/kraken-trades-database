using System;
using KrakenTradesDatabase;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KrakenTradesDatabase.Migrations
{
    public partial class dbsetcandle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:candle_unit", "year,month,week,day,hour,minute")
                .Annotation("Npgsql:Enum:order_side", "buy,sell")
                .Annotation("Npgsql:Enum:order_type_minimal", "limit,market")
                .OldAnnotation("Npgsql:Enum:order_side", "buy,sell")
                .OldAnnotation("Npgsql:Enum:order_type_minimal", "limit,market");

            migrationBuilder.CreateTable(
                name: "Candles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SymbolId = table.Column<int>(type: "integer", nullable: false),
                    DateTimeOffset = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    High = table.Column<decimal>(type: "numeric", nullable: false),
                    Low = table.Column<decimal>(type: "numeric", nullable: false),
                    Open = table.Column<decimal>(type: "numeric", nullable: false),
                    Close = table.Column<decimal>(type: "numeric", nullable: false),
                    CandleUnit = table.Column<CandleUnit>(type: "candle_unit", nullable: false),
                    Interval = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Candles_Symbols_SymbolId",
                        column: x => x.SymbolId,
                        principalTable: "Symbols",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Candles_SymbolId",
                table: "Candles",
                column: "SymbolId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Candles");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:order_side", "buy,sell")
                .Annotation("Npgsql:Enum:order_type_minimal", "limit,market")
                .OldAnnotation("Npgsql:Enum:candle_unit", "year,month,week,day,hour,minute")
                .OldAnnotation("Npgsql:Enum:order_side", "buy,sell")
                .OldAnnotation("Npgsql:Enum:order_type_minimal", "limit,market");
        }
    }
}
