using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SistemaDeGestionDeTicketsAereos.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFareBundlesFromBaggageType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Importante: NO se eliminan físicamente si ya existen registros en Baggage que referencian estos IDs
            // (FK ON DELETE RESTRICT). El requerimiento es que no aparezcan en el módulo de equipaje.
            // La UI ya los filtra; aquí solo reforzamos que queden inactivos si existen.
            migrationBuilder.Sql("""
                UPDATE `BaggageType`
                SET `IsActive` = 0
                WHERE `IdBaggageType` IN (4, 5, 6);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No-op: si la fila existe, ya estaba inactiva.
        }
    }
}
