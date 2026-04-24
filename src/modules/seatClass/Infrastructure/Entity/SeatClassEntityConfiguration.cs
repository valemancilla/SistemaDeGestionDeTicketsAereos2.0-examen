using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Infrastructure.Entity;

// Configuración de EF Core para la tabla SeatClass
public sealed class SeatClassEntityConfiguration : IEntityTypeConfiguration<SeatClassEntity>
{
    public void Configure(EntityTypeBuilder<SeatClassEntity> builder)
    {
        builder.ToTable("SeatClass");

        // Clave primaria
        builder.HasKey(x => x.IdClase);

        // Se genera automáticamente
        builder.Property(x => x.IdClase)
            .HasColumnName("IdClase")
            .ValueGeneratedOnAdd();

        // Nombre de la clase de asiento, obligatorio, máximo 50 caracteres
        builder.Property(x => x.ClassName)
            .HasColumnName("ClassName")
            .HasColumnType("varchar(50)")
            .IsRequired();

        builder.HasData(
            new SeatClassEntity { IdClase = 1, ClassName = "Económica" },
            new SeatClassEntity { IdClase = 2, ClassName = "Ejecutiva" },
            new SeatClassEntity { IdClase = 3, ClassName = "Primera Clase" }
        );
    }
}
