using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaDeGestionDeTicketsAereos.Migrations
{
    /// <inheritdoc />
    public partial class BackfillIssuedTicketsForPaidBookings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Backfill: toda reserva en estado "Pagada" => tiquete(s) pasan a "Emitido"
            // Regla de seguridad: solo promover "Activo" -> "Emitido" para no tocar check-in/abordado/etc.
            migrationBuilder.Sql("""
                UPDATE `Ticket` t
                JOIN `Booking` b ON b.`IdBooking` = t.`IdBooking`
                JOIN `SystemStatus` sb ON sb.`IdStatus` = b.`IdStatus`
                JOIN `SystemStatus` st ON st.`IdStatus` = t.`IdStatus`
                JOIN `SystemStatus` stIssued ON stIssued.`EntityType` = 'Ticket' AND stIssued.`StatusName` = 'Emitido'
                SET t.`IdStatus` = stIssued.`IdStatus`
                WHERE sb.`EntityType` = 'Booking'
                  AND sb.`StatusName` = 'Pagada'
                  AND st.`EntityType` = 'Ticket'
                  AND st.`StatusName` = 'Activo';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
