using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeGestionDeTicketsAereos.src.modules.country.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Infrastructure.Entity;

// Configuración de EF Core para mapear AerolineEntity a la tabla Airline
public sealed class AerolineEntityConfiguration : IEntityTypeConfiguration<AerolineEntity>
{
    public void Configure(EntityTypeBuilder<AerolineEntity> builder)
    {
        // La tabla en BD se llama "Airline"
        builder.ToTable("Airline");

        // Clave primaria
        builder.HasKey(x => x.IdAirline);

        // El ID se genera automáticamente
        builder.Property(x => x.IdAirline)
            .HasColumnName("IdAirline")
            .ValueGeneratedOnAdd();

        // Nombre de la aerolínea, obligatorio, máximo 150 caracteres
        builder.Property(x => x.Name)
            .HasColumnName("Name")
            .HasColumnType("varchar(150)")
            .IsRequired();

        // Código IATA de exactamente 2 letras (ej: AV, LA, AA)
        builder.Property(x => x.IATACode)
            .HasColumnName("IATACode")
            .HasColumnType("char(2)")
            .IsRequired();

        // El código IATA debe ser único en todo el sistema
        builder.HasIndex(x => x.IATACode)
            .IsUnique()
            .HasDatabaseName("UQ_Airline_IATACode");

        // FK al país, es obligatoria
        builder.Property(x => x.IdCountry)
            .HasColumnName("IdCountry")
            .IsRequired();

        // Active por defecto es true cuando se crea la aerolínea
        builder.Property(x => x.Active)
            .HasColumnName("Active")
            .HasColumnType("tinyint(1)")
            .HasDefaultValue(true)
            .IsRequired();

        // Relación: una aerolínea pertenece a un país
        // Restrict: no se puede borrar un país si tiene aerolíneas registradas
        builder.HasOne(x => x.Country)
            .WithMany(x => x.Airlines)
            .HasForeignKey(x => x.IdCountry)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new AerolineEntity
            {
                IdAirline = 1,
                Name = "Aero Demo S.A.",
                IATACode = "DM",
                IdCountry = 1,
                Active = true
            });
    }
}
