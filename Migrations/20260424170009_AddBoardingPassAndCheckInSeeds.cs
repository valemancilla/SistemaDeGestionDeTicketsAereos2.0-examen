using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SistemaDeGestionDeTicketsAereos.Migrations
{
    /// <inheritdoc />
    public partial class AddBoardingPassAndCheckInSeeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BoardingPass",
                columns: table => new
                {
                    IdBoardingPass = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "varchar(25)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdTicket = table.Column<int>(type: "int", nullable: false),
                    IdSeat = table.Column<int>(type: "int", nullable: false),
                    Gate = table.Column<string>(type: "varchar(10)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BoardingTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IdStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardingPass", x => x.IdBoardingPass);
                    table.ForeignKey(
                        name: "FK_BoardingPass_Seat_IdSeat",
                        column: x => x.IdSeat,
                        principalTable: "Seat",
                        principalColumn: "IdSeat",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BoardingPass_SystemStatus_IdStatus",
                        column: x => x.IdStatus,
                        principalTable: "SystemStatus",
                        principalColumn: "IdStatus",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BoardingPass_Ticket_IdTicket",
                        column: x => x.IdTicket,
                        principalTable: "Ticket",
                        principalColumn: "IdTicket",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "ClientFareBundleDisplay",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "BasicBodyMarkup", "ClassicBodyMarkup", "FlexBodyMarkup" },
                values: new object[] { "[bold]Incluye[/]\n[#db2777]✓[/] 1 artículo personal (bolso)\n[#db2777]✓[/] Acumula 3 millas por USD\n[grey]$ Equipaje de mano (10 kg) - Desde {{CARRYON}}[/]\n[grey]$ Equipaje de bodega (23 kg) - Desde {{CHECKED}}[/]\n[grey]$ Check-in en aeropuerto[/]\n[grey]$ Selección de asientos - Desde {{SEAT}}[/]\n[grey]$ Menú a bordo[/]\n[grey]$ Cambios antes del vuelo[/]\n[grey]✗ Reembolsos antes del vuelo[/]", "[bold]Incluye[/]\n[#6d28d9]✓[/] 1 artículo personal (bolso)\n[#6d28d9]✓[/] 1 equipaje de mano (10 kg)\n[#6d28d9]✓[/] 1 equipaje de bodega (23 kg)\n[#6d28d9]✓[/] Check-in en aeropuerto\n[#6d28d9]✓[/] Asiento Economy incluido\n[#6d28d9]✓[/] Acumula 6 millas por USD\n[grey]$ Menú a bordo[/]\n[grey]$ Cambios antes del vuelo[/]\n[grey]✗ Reembolsos antes del vuelo[/]", "[bold]Incluye[/]\n[#ea580c]✓[/] 1 artículo personal (bolso)\n[#ea580c]✓[/] 1 equipaje de mano (10 kg)\n[#ea580c]✓[/] 1 equipaje de bodega (23 kg)\n[#ea580c]✓[/] Check-in en aeropuerto\n[#ea580c]✓[/] Asiento Plus\n[#ea580c]✓[/] Acumula 8 millas por USD\n[#ea580c]✓[/] Cambios antes del vuelo\n[#ea580c]✓[/] Reembolsos antes del vuelo\n[grey]$ Menú a bordo[/]" });

            migrationBuilder.InsertData(
                table: "SystemStatus",
                columns: new[] { "IdStatus", "EntityType", "StatusName" },
                values: new object[,]
                {
                    { 21, "Ticket", "Emitido" },
                    { 22, "Ticket", "Check-in realizado" },
                    { 23, "BoardingPass", "Generado" },
                    { 24, "BoardingPass", "Activo" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BoardingPass_Code",
                table: "BoardingPass",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BoardingPass_IdSeat",
                table: "BoardingPass",
                column: "IdSeat");

            migrationBuilder.CreateIndex(
                name: "IX_BoardingPass_IdStatus",
                table: "BoardingPass",
                column: "IdStatus");

            migrationBuilder.CreateIndex(
                name: "IX_BoardingPass_IdTicket",
                table: "BoardingPass",
                column: "IdTicket",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BoardingPass");

            migrationBuilder.DeleteData(
                table: "SystemStatus",
                keyColumn: "IdStatus",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "SystemStatus",
                keyColumn: "IdStatus",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "SystemStatus",
                keyColumn: "IdStatus",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "SystemStatus",
                keyColumn: "IdStatus",
                keyValue: 24);

            migrationBuilder.UpdateData(
                table: "ClientFareBundleDisplay",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "BasicBodyMarkup", "ClassicBodyMarkup", "FlexBodyMarkup" },
                values: new object[] { "[bold]Incluye[/]\r\n[#db2777]✓[/] 1 artículo personal (bolso)\r\n[#db2777]✓[/] Acumula 3 millas por USD\r\n[grey]$ Equipaje de mano (10 kg) - Desde {{CARRYON}}[/]\r\n[grey]$ Equipaje de bodega (23 kg) - Desde {{CHECKED}}[/]\r\n[grey]$ Check-in en aeropuerto[/]\r\n[grey]$ Selección de asientos - Desde {{SEAT}}[/]\r\n[grey]$ Menú a bordo[/]\r\n[grey]$ Cambios antes del vuelo[/]\r\n[grey]✗ Reembolsos antes del vuelo[/]", "[bold]Incluye[/]\r\n[#6d28d9]✓[/] 1 artículo personal (bolso)\r\n[#6d28d9]✓[/] 1 equipaje de mano (10 kg)\r\n[#6d28d9]✓[/] 1 equipaje de bodega (23 kg)\r\n[#6d28d9]✓[/] Check-in en aeropuerto\r\n[#6d28d9]✓[/] Asiento Economy incluido\r\n[#6d28d9]✓[/] Acumula 6 millas por USD\r\n[grey]$ Menú a bordo[/]\r\n[grey]$ Cambios antes del vuelo[/]\r\n[grey]✗ Reembolsos antes del vuelo[/]", "[bold]Incluye[/]\r\n[#ea580c]✓[/] 1 artículo personal (bolso)\r\n[#ea580c]✓[/] 1 equipaje de mano (10 kg)\r\n[#ea580c]✓[/] 1 equipaje de bodega (23 kg)\r\n[#ea580c]✓[/] Check-in en aeropuerto\r\n[#ea580c]✓[/] Asiento Plus\r\n[#ea580c]✓[/] Acumula 8 millas por USD\r\n[#ea580c]✓[/] Cambios antes del vuelo\r\n[#ea580c]✓[/] Reembolsos antes del vuelo\r\n[grey]$ Menú a bordo[/]" });
        }
    }
}
