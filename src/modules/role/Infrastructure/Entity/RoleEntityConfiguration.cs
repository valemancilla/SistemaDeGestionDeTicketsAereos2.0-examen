using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SistemaDeGestionDeTicketsAereos.src.modules.role.Infrastructure.Entity;

// Configuración de EF Core para mapear RoleEntity a la tabla Role
public sealed class RoleEntityConfiguration : IEntityTypeConfiguration<RoleEntity>
{
    public void Configure(EntityTypeBuilder<RoleEntity> builder)
    {
        builder.ToTable("Role");

        // Clave primaria
        builder.HasKey(x => x.IdUserRole);

        // El ID se genera automáticamente
        builder.Property(x => x.IdUserRole)
            .HasColumnName("IdUserRole")
            .ValueGeneratedOnAdd();

        // Nombre del rol, obligatorio, máximo 50 caracteres
        builder.Property(x => x.RoleName)
            .HasColumnName("RoleName")
            .HasColumnType("varchar(50)")
            .IsRequired();

        // Data Seeding: insertamos los roles base directamente desde el código
        // Así no hay que hacerlo manualmente en la BD cada vez que se migra
        builder.HasData(
            new RoleEntity { IdUserRole = 1, RoleName = "Administrador" },
            new RoleEntity { IdUserRole = 2, RoleName = "Cliente" }
        );
    }
}
