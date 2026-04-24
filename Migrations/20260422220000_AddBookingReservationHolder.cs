using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaDeGestionDeTicketsAereos.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingReservationHolder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ConsentDataProcessing",
                table: "Booking",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ConsentMarketing",
                table: "Booking",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "HolderEmail",
                table: "Booking",
                type: "varchar(160)",
                maxLength: 160,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "HolderPhone",
                table: "Booking",
                type: "varchar(32)",
                maxLength: 32,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "HolderPhonePrefix",
                table: "Booking",
                type: "varchar(8)",
                maxLength: 8,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "IdHolderPerson",
                table: "Booking",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Booking_IdHolderPerson",
                table: "Booking",
                column: "IdHolderPerson");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_Person_IdHolderPerson",
                table: "Booking",
                column: "IdHolderPerson",
                principalTable: "Person",
                principalColumn: "IdPerson",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_Person_IdHolderPerson",
                table: "Booking");

            migrationBuilder.DropIndex(
                name: "IX_Booking_IdHolderPerson",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "ConsentDataProcessing",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "ConsentMarketing",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "HolderEmail",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "HolderPhone",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "HolderPhonePrefix",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "IdHolderPerson",
                table: "Booking");
        }
    }
}
