using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaDeGestionDeTicketsAereos.Migrations
{
    /// <inheritdoc />
    public partial class SeedClientFareBundleBaggageTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "BaggageType",
                columns: new[] { "IdBaggageType", "TypeName" },
                values: new object[,]
                {
                    { 4, "Tarifa Basic (elegida al comprar)" },
                    { 5, "Tarifa Classic (elegida al comprar)" },
                    { 6, "Tarifa Flex (elegida al comprar)" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BaggageType",
                keyColumn: "IdBaggageType",
                keyValue: 4);
            migrationBuilder.DeleteData(
                table: "BaggageType",
                keyColumn: "IdBaggageType",
                keyValue: 5);
            migrationBuilder.DeleteData(
                table: "BaggageType",
                keyColumn: "IdBaggageType",
                keyValue: 6);
        }
    }
}
