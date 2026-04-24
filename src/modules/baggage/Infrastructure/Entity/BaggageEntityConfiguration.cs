using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.baggage.Infrastructure.Entity;

// Configuración de EF Core para mapear BaggageEntity a la tabla Baggage
public sealed class BaggageEntityConfiguration : IEntityTypeConfiguration<BaggageEntity>
{
    public void Configure(EntityTypeBuilder<BaggageEntity> builder)
    {
        builder.ToTable("Baggage");

        // Clave primaria
        builder.HasKey(x => x.IdBaggage);

        // El ID se genera automáticamente
        builder.Property(x => x.IdBaggage)
            .HasColumnName("IdBaggage")
            .ValueGeneratedOnAdd();

        // FK al ticket, obligatoria
        builder.Property(x => x.IdTicket)
            .HasColumnName("IdTicket")
            .IsRequired();

        // FK al tipo de equipaje, obligatoria
        builder.Property(x => x.IdBaggageType)
            .HasColumnName("IdBaggageType")
            .IsRequired();

        // Peso con hasta 6 dígitos en total y 2 decimales (ej: 2345.67 kg)
        builder.Property(x => x.Weight)
            .HasColumnName("Weight")
            .HasColumnType("decimal(6,2)")
            .IsRequired();

        // Relación: un equipaje pertenece a un ticket, un ticket puede tener varios equipajes
        // Restrict: no se puede borrar un ticket si tiene equipajes registrados
        builder.HasOne(x => x.Ticket)
            .WithMany(x => x.Baggages)
            .HasForeignKey(x => x.IdTicket)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación: un equipaje tiene un tipo, un tipo puede estar en muchos equipajes
        // Restrict: no se puede borrar un tipo de equipaje si hay equipajes con ese tipo
        builder.HasOne(x => x.BaggageType)
            .WithMany(x => x.Baggages)
            .HasForeignKey(x => x.IdBaggageType)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
