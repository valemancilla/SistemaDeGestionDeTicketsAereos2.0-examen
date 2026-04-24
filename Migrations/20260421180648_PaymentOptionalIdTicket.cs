using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaDeGestionDeTicketsAereos.Migrations
{
    /// <inheritdoc />
    public partial class PaymentOptionalIdTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdTicket",
                table: "Payment",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payment_IdTicket",
                table: "Payment",
                column: "IdTicket");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Ticket_IdTicket",
                table: "Payment",
                column: "IdTicket",
                principalTable: "Ticket",
                principalColumn: "IdTicket",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Ticket_IdTicket",
                table: "Payment");

            migrationBuilder.DropIndex(
                name: "IX_Payment_IdTicket",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "IdTicket",
                table: "Payment");
        }
    }
}
