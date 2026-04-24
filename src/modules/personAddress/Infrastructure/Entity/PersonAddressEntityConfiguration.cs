using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeGestionDeTicketsAereos.src.modules.city.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Infrastructure.Entity;

// Configuración de EF Core para mapear PersonAddressEntity a la tabla PersonAddress
public sealed class PersonAddressEntityConfiguration : IEntityTypeConfiguration<PersonAddressEntity>
{
    public void Configure(EntityTypeBuilder<PersonAddressEntity> builder)
    {
        builder.ToTable("PersonAddress");

        // Clave primaria
        builder.HasKey(x => x.IdAddress);

        // El ID se genera automáticamente
        builder.Property(x => x.IdAddress)
            .HasColumnName("IdAddress")
            .ValueGeneratedOnAdd();

        // FK a la persona, obligatoria
        builder.Property(x => x.IdPerson)
            .HasColumnName("IdPerson")
            .IsRequired();

        builder.Property(x => x.Street)
            .HasColumnName("Street")
            .HasColumnType("varchar(250)")
            .IsRequired();

        builder.Property(x => x.Number)
            .HasColumnName("Number")
            .HasColumnType("varchar(50)")
            .IsRequired();

        builder.Property(x => x.Neighborhood)
            .HasColumnName("Neighborhood")
            .HasColumnType("varchar(120)")
            .IsRequired();

        builder.Property(x => x.DwellingType)
            .HasColumnName("DwellingType")
            .HasColumnType("varchar(20)")
            .IsRequired();

        // FK a la ciudad, obligatoria
        builder.Property(x => x.IdCity)
            .HasColumnName("IdCity")
            .IsRequired();

        // Código postal, opcional, máximo 20 caracteres
        builder.Property(x => x.ZipCode)
            .HasColumnName("ZipCode")
            .HasColumnType("varchar(20)");

        // Active por defecto es true cuando se registra la dirección
        builder.Property(x => x.Active)
            .HasColumnName("Active")
            .HasColumnType("tinyint(1)")
            .HasDefaultValue(true)
            .IsRequired();

        // Relación: una dirección pertenece a una persona, una persona puede tener varias direcciones
        builder.HasOne(x => x.Person)
            .WithMany(x => x.Addresses)
            .HasForeignKey(x => x.IdPerson)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación: una dirección está en una ciudad específica
        builder.HasOne(x => x.City)
            .WithMany(x => x.PersonAddresses)
            .HasForeignKey(x => x.IdCity)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
