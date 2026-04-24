using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaDeGestionDeTicketsAereos.Migrations
{
    /// <inheritdoc />
    public partial class UniquePersonDocumentNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Alinear con la normalización del dominio (PersonDocumentNumber: trim + mayúsculas).
            // Si existían dos filas que solo difieren por mayúsculas, esta sentencia deja un duplicado y el índice fallará: hay que unificar a mano y volver a migrar.
            migrationBuilder.Sql(
                "UPDATE `Person` SET `DocumentNumber` = UPPER(TRIM(`DocumentNumber`)) WHERE `DocumentNumber` IS NOT NULL;");

            migrationBuilder.CreateIndex(
                name: "UQ_Person_DocumentNumber",
                table: "Person",
                column: "DocumentNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UQ_Person_DocumentNumber",
                table: "Person");
        }
    }
}
