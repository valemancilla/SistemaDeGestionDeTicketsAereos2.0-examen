using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Infrastructure.Entity;

// Configuración de EF Core para mapear AircraftEntity a la tabla Aircraft
public sealed class AircraftEntityConfiguration : IEntityTypeConfiguration<AircraftEntity>
{
    public void Configure(EntityTypeBuilder<AircraftEntity> builder)
    {
        builder.ToTable("Aircraft");

        // Clave primaria
        builder.HasKey(x => x.IdAircraft);

        // El ID se genera automáticamente
        builder.Property(x => x.IdAircraft)
            .HasColumnName("IdAircraft")
            .ValueGeneratedOnAdd();

        // FK a la aerolínea, obligatoria
        builder.Property(x => x.IdAirline)
            .HasColumnName("IdAirline")
            .IsRequired();

        // FK al modelo, obligatoria
        builder.Property(x => x.IdModel)
            .HasColumnName("IdModel")
            .IsRequired();

        // Capacidad total del avión, obligatoria
        builder.Property(x => x.Capacity)
            .HasColumnName("Capacity")
            .IsRequired();

        // Relación: un avión tiene un modelo, un modelo puede estar en muchos aviones
        builder.HasOne(x => x.Model)
            .WithMany(x => x.Aircrafts)
            .HasForeignKey(x => x.IdModel)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación: un avión pertenece a una aerolínea, una aerolínea tiene muchos aviones
        builder.HasOne(x => x.Airline)
            .WithMany(x => x.Aircrafts)
            .HasForeignKey(x => x.IdAirline)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new AircraftEntity { IdAircraft = 1, IdAirline = 1, IdModel = 1, Capacity = 20 });
    }
}
