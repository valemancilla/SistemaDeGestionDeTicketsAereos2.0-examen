using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.fare.Infrastructure.Entity;

public sealed class FareSeatClassPriceEntityConfiguration : IEntityTypeConfiguration<FareSeatClassPriceEntity>
{
    public void Configure(EntityTypeBuilder<FareSeatClassPriceEntity> builder)
    {
        builder.ToTable("FareSeatClassPrice");

        builder.HasKey(x => new { x.IdFare, x.IdClase });

        builder.Property(x => x.IdFare)
            .HasColumnName("IdFare")
            .IsRequired();

        builder.Property(x => x.IdClase)
            .HasColumnName("IdClase")
            .IsRequired();

        builder.Property(x => x.Price)
            .HasColumnName("Price")
            .HasColumnType("decimal(12,2)")
            .IsRequired();

        builder.HasOne(x => x.Fare)
            .WithMany(x => x.SeatClassPrices)
            .HasForeignKey(x => x.IdFare)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.SeatClass)
            .WithMany()
            .HasForeignKey(x => x.IdClase)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new FareSeatClassPriceEntity { IdFare = 1, IdClase = 1, Price = 380_000m },
            new FareSeatClassPriceEntity { IdFare = 1, IdClase = 2, Price = 620_000m },
            new FareSeatClassPriceEntity { IdFare = 1, IdClase = 3, Price = 1_200_000m });
    }
}

