using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaDeGestionDeTicketsAereos.Migrations
{
    /// <inheritdoc />
    public partial class AddBoardingGateAndTicketAbordado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BoardingGate",
                table: "Flight",
                type: "varchar(20)",
                nullable: false,
                defaultValue: "A01")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Flight",
                keyColumn: "IdFlight",
                keyValue: 1,
                column: "BoardingGate",
                value: "B12");

            migrationBuilder.InsertData(
                table: "SystemStatus",
                columns: new[] { "IdStatus", "EntityType", "StatusName" },
                values: new object[] { 25, "Ticket", "Abordado" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SystemStatus",
                keyColumn: "IdStatus",
                keyValue: 25);

            migrationBuilder.DropColumn(
                name: "BoardingGate",
                table: "Flight");
        }
    }
}
