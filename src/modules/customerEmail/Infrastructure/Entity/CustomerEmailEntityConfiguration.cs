using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.customerEmail.Infrastructure.Entity;

// Configuración de EF Core para mapear CustomerEmailEntity a la tabla CustomerEmail
public sealed class CustomerEmailEntityConfiguration : IEntityTypeConfiguration<CustomerEmailEntity>
{
    public void Configure(EntityTypeBuilder<CustomerEmailEntity> builder)
    {
        builder.ToTable("CustomerEmail");

        // Clave primaria
        builder.HasKey(x => x.IdEmail);

        // El ID se genera automáticamente
        builder.Property(x => x.IdEmail)
            .HasColumnName("IdEmail")
            .ValueGeneratedOnAdd();

        // FK a la persona, obligatoria
        builder.Property(x => x.IdPerson)
            .HasColumnName("IdPerson")
            .IsRequired();

        // Correo electrónico, obligatorio, máximo 150 caracteres
        builder.Property(x => x.Email)
            .HasColumnName("Email")
            .HasColumnType("varchar(150)")
            .IsRequired();

        // Relación: un email pertenece a una persona, una persona puede tener varios emails
        // Restrict: no se puede borrar una persona si tiene correos registrados
        builder.HasOne(x => x.Person)
            .WithMany(x => x.Emails)
            .HasForeignKey(x => x.IdPerson)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
