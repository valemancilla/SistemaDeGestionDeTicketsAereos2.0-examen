using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeGestionDeTicketsAereos.src.shared.helpers;

namespace SistemaDeGestionDeTicketsAereos.src.modules.clientFareBundleDisplay.Infrastructure.Entity;

public sealed class ClientFareBundleDisplayEntityConfiguration : IEntityTypeConfiguration<ClientFareBundleDisplayEntity>
{
    public void Configure(EntityTypeBuilder<ClientFareBundleDisplayEntity> builder)
    {
        builder.ToTable("ClientFareBundleDisplay");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.RefCarryOnCop).HasColumnType("decimal(12,2)").IsRequired();
        builder.Property(x => x.RefCheckedCop).HasColumnType("decimal(12,2)").IsRequired();
        builder.Property(x => x.ClassicMultiplier).HasColumnType("decimal(9,4)").IsRequired();
        builder.Property(x => x.FlexMultiplier).HasColumnType("decimal(9,4)").IsRequired();
        builder.Property(x => x.UnpublishedFareReferenceCop).HasColumnType("decimal(12,2)").IsRequired();
        builder.Property(x => x.SeatSelectionFromCop).HasColumnType("decimal(12,2)").IsRequired();

        builder.Property(x => x.SubtitleLine).HasColumnType("varchar(500)").IsRequired();
        builder.Property(x => x.ExplainerLine).HasColumnType("varchar(1000)").IsRequired();
        builder.Property(x => x.BasicBodyMarkup).HasColumnType("longtext").IsRequired();
        builder.Property(x => x.ClassicBodyMarkup).HasColumnType("longtext").IsRequired();
        builder.Property(x => x.FlexBodyMarkup).HasColumnType("longtext").IsRequired();

        builder.HasData(
            new ClientFareBundleDisplayEntity
            {
                Id = 1,
                RefCarryOnCop = ClientFareBundleDisplayDefaults.ReferenceCarryOnCop,
                RefCheckedCop = ClientFareBundleDisplayDefaults.ReferenceCheckedCop,
                ClassicMultiplier = ClientFareBundleDisplayDefaults.ClassicMultiplier,
                FlexMultiplier = ClientFareBundleDisplayDefaults.FlexMultiplier,
                UnpublishedFareReferenceCop = ClientFareBundleDisplayDefaults.UnpublishedFareReferenceCop,
                SeatSelectionFromCop = ClientFareBundleDisplayDefaults.SeatSelectionFromCop,
                SubtitleLine = ClientFareBundleDisplayDefaults.SubtitleLine,
                ExplainerLine = ClientFareBundleDisplayDefaults.ExplainerLineTemplate,
                BasicBodyMarkup = ClientFareBundleDisplayDefaults.BasicBody(),
                ClassicBodyMarkup = ClientFareBundleDisplayDefaults.ClassicBody,
                FlexBodyMarkup = ClientFareBundleDisplayDefaults.FlexBody
            });
    }
}
