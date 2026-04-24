using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaDeGestionDeTicketsAereos.Migrations
{
    /// <inheritdoc />
    public partial class AddClientFareBundleDisplay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClientFareBundleDisplay",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    RefCarryOnCop = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    RefCheckedCop = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    ClassicMultiplier = table.Column<decimal>(type: "decimal(9,4)", nullable: false),
                    FlexMultiplier = table.Column<decimal>(type: "decimal(9,4)", nullable: false),
                    UnpublishedFareReferenceCop = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    SubtitleLine = table.Column<string>(type: "varchar(500)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExplainerLine = table.Column<string>(type: "varchar(1000)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BasicBodyMarkup = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClassicBodyMarkup = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FlexBodyMarkup = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SeatSelectionFromCop = table.Column<decimal>(type: "decimal(12,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientFareBundleDisplay", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientFareBundleDisplay");
        }
    }
}
