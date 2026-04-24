using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaDeGestionDeTicketsAereos.Migrations
{
    /// <inheritdoc />
    public partial class AddFareSeatClassPrices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FareSeatClassPrice",
                columns: table => new
                {
                    IdFare = table.Column<int>(type: "int", nullable: false),
                    IdClase = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(12,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FareSeatClassPrice", x => new { x.IdFare, x.IdClase });
                    table.ForeignKey(
                        name: "FK_FareSeatClassPrice_Fare_IdFare",
                        column: x => x.IdFare,
                        principalTable: "Fare",
                        principalColumn: "IdFare",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FareSeatClassPrice_SeatClass_IdClase",
                        column: x => x.IdClase,
                        principalTable: "SeatClass",
                        principalColumn: "IdClase",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_FareSeatClassPrice_IdClase",
                table: "FareSeatClassPrice",
                column: "IdClase");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FareSeatClassPrice");
        }
    }
}
