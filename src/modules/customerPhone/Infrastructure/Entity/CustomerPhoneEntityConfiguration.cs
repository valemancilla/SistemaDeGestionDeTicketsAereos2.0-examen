using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.customerPhone.Infrastructure.Entity;

// Configuración de EF Core para mapear CustomerPhoneEntity a la tabla CustomerPhone
public sealed class CustomerPhoneEntityConfiguration : IEntityTypeConfiguration<CustomerPhoneEntity>
{
    public void Configure(EntityTypeBuilder<CustomerPhoneEntity> builder)
    {
        builder.ToTable("CustomerPhone");

        // Clave primaria
        builder.HasKey(x => x.IdPhone);

        // El ID se genera automáticamente
        builder.Property(x => x.IdPhone)
            .HasColumnName("IdPhone")
            .ValueGeneratedOnAdd();

        // FK a la persona, obligatoria
        builder.Property(x => x.IdPerson)
            .HasColumnName("IdPerson")
            .IsRequired();

        // Número de teléfono, obligatorio, máximo 20 caracteres para admitir código de país
        builder.Property(x => x.Phone)
            .HasColumnName("Phone")
            .HasColumnType("varchar(20)")
            .IsRequired();

        // Relación: un teléfono pertenece a una persona, una persona puede tener varios teléfonos
        // Restrict: no se puede borrar una persona si tiene teléfonos registrados
        builder.HasOne(x => x.Person)
            .WithMany(x => x.Phones)
            .HasForeignKey(x => x.IdPerson)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
