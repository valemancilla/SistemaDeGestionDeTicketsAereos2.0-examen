using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.customer.Infrastructure.Entity;

// Configuración de EF Core para mapear CustomerEntity a la tabla Customer
public sealed class CustomerEntityConfiguration : IEntityTypeConfiguration<CustomerEntity>
{
    public void Configure(EntityTypeBuilder<CustomerEntity> builder)
    {
        builder.ToTable("Customer");

        // Clave primaria
        builder.HasKey(x => x.IdCustomer);

        // El ID se genera automáticamente
        builder.Property(x => x.IdCustomer)
            .HasColumnName("IdCustomer")
            .ValueGeneratedOnAdd();

        // FK a la persona, obligatoria
        builder.Property(x => x.IdPerson)
            .HasColumnName("IdPerson")
            .IsRequired();

        // Cada cliente debe estar vinculado a una sola persona (relación 1 a 1)
        builder.HasIndex(x => x.IdPerson)
            .IsUnique()
            .HasDatabaseName("UQ_Customer_Person");

        // Active por defecto es true cuando se registra el cliente
        builder.Property(x => x.Active)
            .HasColumnName("Active")
            .HasColumnType("tinyint(1)")
            .HasDefaultValue(true)
            .IsRequired();

        // Fecha de registro del cliente, solo fecha sin hora
        builder.Property(x => x.RegistrationDate)
            .HasColumnName("RegistrationDate")
            .HasColumnType("date")
            .IsRequired();

        // Relación uno a uno: un cliente tiene exactamente una persona asociada
        // Restrict: no se puede borrar una persona si tiene un cliente vinculado
        builder.HasOne(x => x.Person)
            .WithOne(x => x.Customer)
            .HasForeignKey<CustomerEntity>(x => x.IdPerson)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
