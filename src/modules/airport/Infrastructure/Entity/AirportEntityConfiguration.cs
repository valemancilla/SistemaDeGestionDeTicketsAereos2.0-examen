using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeGestionDeTicketsAereos.src.modules.city.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.airport.Infrastructure.Entity;

// Configuración de EF Core para mapear AirportEntity a la tabla Airport
public sealed class AirportEntityConfiguration : IEntityTypeConfiguration<AirportEntity>
{
    public void Configure(EntityTypeBuilder<AirportEntity> builder)
    {
        builder.ToTable("Airport");

        // Clave primaria
        builder.HasKey(x => x.IdAirport);

        // El ID se genera automáticamente
        builder.Property(x => x.IdAirport)
            .HasColumnName("IdAirport")
            .ValueGeneratedOnAdd();

        // Nombre del aeropuerto, obligatorio, máximo 150 caracteres
        builder.Property(x => x.Name)
            .HasColumnName("Name")
            .HasColumnType("varchar(150)")
            .IsRequired();

        // Código IATA de exactamente 3 letras (ej: BOG, MIA, JFK)
        builder.Property(x => x.IATACode)
            .HasColumnName("IATACode")
            .HasColumnType("char(3)")
            .IsRequired();

        // FK a la ciudad, obligatoria
        builder.Property(x => x.IdCity)
            .HasColumnName("IdCity")
            .IsRequired();

        // Active por defecto es true cuando se registra el aeropuerto
        builder.Property(x => x.Active)
            .HasColumnName("Active")
            .HasColumnType("tinyint(1)")
            .HasDefaultValue(true)
            .IsRequired();

        // El código IATA debe ser único, no puede haber dos aeropuertos con el mismo código
        builder.HasIndex(x => x.IATACode)
            .IsUnique()
            .HasDatabaseName("UQ_Airport_IATACode");

        // Relación: un aeropuerto pertenece a una ciudad, una ciudad puede tener varios aeropuertos
        // Restrict: no se puede borrar una ciudad si tiene aeropuertos registrados
        builder.HasOne(x => x.City)
            .WithMany(x => x.Airports)
            .HasForeignKey(x => x.IdCity)
            .OnDelete(DeleteBehavior.Restrict);

        // IdCity alineados con CityEntityConfiguration (Bogotá=1, Medellín=2, Miami=19).
        builder.HasData(
            new AirportEntity { IdAirport = 1, Name = "Aeropuerto Internacional El Dorado", IATACode = "BOG", IdCity = 1, Active = true },
            new AirportEntity { IdAirport = 2, Name = "Aeropuerto Internacional José María Córdova", IATACode = "MDE", IdCity = 2, Active = true },
            new AirportEntity { IdAirport = 3, Name = "Miami International Airport", IATACode = "MIA", IdCity = 19, Active = true });
    }
}
