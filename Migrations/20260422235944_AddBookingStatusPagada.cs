using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaDeGestionDeTicketsAereos.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingStatusPagada : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Idempotente: el estado «Pagada» también puede venir del seed del modelo (SystemStatusEntityConfiguration).
            migrationBuilder.Sql("""
                INSERT INTO `SystemStatus` (`IdStatus`, `EntityType`, `StatusName`)
                SELECT 20, 'Booking', 'Pagada'
                WHERE NOT EXISTS (SELECT 1 FROM `SystemStatus` WHERE `IdStatus` = 20 LIMIT 1);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM `SystemStatus` WHERE `IdStatus` = 20 AND `EntityType` = 'Booking' AND `StatusName` = 'Pagada';");
        }
    }
}
