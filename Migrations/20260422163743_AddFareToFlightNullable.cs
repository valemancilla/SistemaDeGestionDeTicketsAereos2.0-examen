using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaDeGestionDeTicketsAereos.Migrations
{
    /// <inheritdoc />
    public partial class AddFareToFlightNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdFare",
                table: "Flight",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Flight_IdFare",
                table: "Flight",
                column: "IdFare");

            migrationBuilder.AddForeignKey(
                name: "FK_Flight_Fare_IdFare",
                table: "Flight",
                column: "IdFare",
                principalTable: "Fare",
                principalColumn: "IdFare",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flight_Fare_IdFare",
                table: "Flight");

            migrationBuilder.DropIndex(
                name: "IX_Flight_IdFare",
                table: "Flight");

            migrationBuilder.DropColumn(
                name: "IdFare",
                table: "Flight");
        }
    }
}
