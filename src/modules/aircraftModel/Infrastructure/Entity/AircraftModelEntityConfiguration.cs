using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Infrastructure.Entity;

// Configuración de EF Core para la tabla AircraftModel
public sealed class AircraftModelEntityConfiguration : IEntityTypeConfiguration<AircraftModelEntity>
{
    public void Configure(EntityTypeBuilder<AircraftModelEntity> builder)
    {
        builder.ToTable("AircraftModel");

        // Clave primaria
        builder.HasKey(x => x.IdModel);

        // Se genera automáticamente
        builder.Property(x => x.IdModel)
            .HasColumnName("IdModel")
            .ValueGeneratedOnAdd();

        // FK al fabricante, es obligatoria
        builder.Property(x => x.IdManufacturer)
            .HasColumnName("IdManufacturer")
            .IsRequired();

        // Nombre del modelo, obligatorio, máximo 50 caracteres
        builder.Property(x => x.Model)
            .HasColumnName("Model")
            .HasColumnType("varchar(50)")
            .IsRequired();

        // Relación: un modelo pertenece a un fabricante, un fabricante tiene muchos modelos
        // Restrict: no se puede borrar un fabricante si tiene modelos registrados
        builder.HasOne(x => x.Manufacturer)
            .WithMany(x => x.AircraftModels)
            .HasForeignKey(x => x.IdManufacturer)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new AircraftModelEntity { IdModel = 1, IdManufacturer = 1, Model = "737-800" });
    }
}
