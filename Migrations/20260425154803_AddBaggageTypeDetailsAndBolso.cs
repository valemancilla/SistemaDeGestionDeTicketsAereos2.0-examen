using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaDeGestionDeTicketsAereos.Migrations
{
    /// <inheritdoc />
    public partial class AddBaggageTypeDetailsAndBolso : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BasePriceCop",
                table: "BaggageType",
                type: "decimal(12,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "BaggageType",
                type: "varchar(500)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "BaggageType",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "WeightKg",
                table: "BaggageType",
                type: "decimal(6,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "BaggageType",
                keyColumn: "IdBaggageType",
                keyValue: 1,
                columns: new[] { "BasePriceCop", "Description", "IsActive", "WeightKg" },
                values: new object[] { 70000m, "Equipaje de mano (10 kg).", true, 10m });

            migrationBuilder.UpdateData(
                table: "BaggageType",
                keyColumn: "IdBaggageType",
                keyValue: 2,
                columns: new[] { "BasePriceCop", "Description", "IsActive", "WeightKg" },
                values: new object[] { 70000m, "Equipaje de bodega (23 kg).", true, 23m });

            migrationBuilder.UpdateData(
                table: "BaggageType",
                keyColumn: "IdBaggageType",
                keyValue: 3,
                columns: new[] { "BasePriceCop", "Description", "IsActive", "WeightKg" },
                values: new object[] { 0m, "Equipaje especial.", true, 0m });

            migrationBuilder.UpdateData(
                table: "BaggageType",
                keyColumn: "IdBaggageType",
                keyValue: 4,
                columns: new[] { "BasePriceCop", "Description", "IsActive", "WeightKg" },
                values: new object[] { 0m, null, false, 0m });

            migrationBuilder.UpdateData(
                table: "BaggageType",
                keyColumn: "IdBaggageType",
                keyValue: 5,
                columns: new[] { "BasePriceCop", "Description", "IsActive", "WeightKg" },
                values: new object[] { 0m, null, false, 0m });

            migrationBuilder.UpdateData(
                table: "BaggageType",
                keyColumn: "IdBaggageType",
                keyValue: 6,
                columns: new[] { "BasePriceCop", "Description", "IsActive", "WeightKg" },
                values: new object[] { 0m, null, false, 0m });

            migrationBuilder.InsertData(
                table: "BaggageType",
                columns: new[] { "IdBaggageType", "BasePriceCop", "Description", "IsActive", "TypeName", "WeightKg" },
                values: new object[] { 7, 0m, "Artículo personal (bolso).", true, "Artículo personal (bolso)", 0m });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BaggageType",
                keyColumn: "IdBaggageType",
                keyValue: 7);

            migrationBuilder.DropColumn(
                name: "BasePriceCop",
                table: "BaggageType");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "BaggageType");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "BaggageType");

            migrationBuilder.DropColumn(
                name: "WeightKg",
                table: "BaggageType");
        }
    }
}
