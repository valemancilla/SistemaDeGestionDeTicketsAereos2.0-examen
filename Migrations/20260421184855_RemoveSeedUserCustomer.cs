using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaDeGestionDeTicketsAereos.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSeedUserCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Quitar primero filas que referencian al usuario semilla IdUser = 2 (evita error de FK al borrar User).
            migrationBuilder.Sql("""
                DELETE FROM `BookingCustomer` WHERE `IdUser` = 2;
                DELETE FROM `BookingCancellation` WHERE `IdUser` = 2;
                DELETE FROM `BookingStatusHistory` WHERE `IdUser` = 2;
                DELETE FROM `TicketStatusHistory` WHERE `IdUser` = 2;
                DELETE FROM `CheckIn` WHERE `IdUser` = 2;
                DELETE FROM `FlightStatusHistory` WHERE `IdUser` = 2;
                DELETE FROM `User` WHERE `IdUser` = 2;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "IdUser", "Active", "IdPerson", "IdUserRole", "Password", "Username" },
                values: new object[] { 2, true, null, 2, "12345678", "customer" });
        }
    }
}
