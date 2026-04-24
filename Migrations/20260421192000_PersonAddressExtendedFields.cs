using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaDeGestionDeTicketsAereos.Migrations
{
    /// <inheritdoc />
    public partial class PersonAddressExtendedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Neighborhood",
                table: "PersonAddress",
                type: "varchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "Sin registrar");

            migrationBuilder.AddColumn<string>(
                name: "DwellingType",
                table: "PersonAddress",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Casa");

            migrationBuilder.AlterColumn<string>(
                name: "Street",
                table: "PersonAddress",
                type: "varchar(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(150)",
                oldMaxLength: 150,
                oldNullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "PersonAddress",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldMaxLength: 20,
                oldNullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Street",
                table: "PersonAddress",
                type: "varchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(250)",
                oldMaxLength: 250,
                oldNullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "PersonAddress",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: false);

            migrationBuilder.DropColumn(
                name: "Neighborhood",
                table: "PersonAddress");

            migrationBuilder.DropColumn(
                name: "DwellingType",
                table: "PersonAddress");
        }
    }
}
