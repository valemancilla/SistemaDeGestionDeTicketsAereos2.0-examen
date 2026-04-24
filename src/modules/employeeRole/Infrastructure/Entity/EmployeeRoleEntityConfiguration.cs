using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Infrastructure.Entity;

// Configuración de EF Core para la tabla EmployeeRole
public sealed class EmployeeRoleEntityConfiguration : IEntityTypeConfiguration<EmployeeRoleEntity>
{
    public void Configure(EntityTypeBuilder<EmployeeRoleEntity> builder)
    {
        builder.ToTable("EmployeeRole");

        // Clave primaria
        builder.HasKey(x => x.IdRole);

        // Se genera automáticamente
        builder.Property(x => x.IdRole)
            .HasColumnName("IdRole")
            .ValueGeneratedOnAdd();

        // Nombre del rol, obligatorio, máximo 80 caracteres
        // (es más largo que otros porque los títulos de empleados pueden ser largos)
        builder.Property(x => x.RoleName)
            .HasColumnName("RoleName")
            .HasColumnType("varchar(80)")
            .IsRequired();

        builder.HasData(
            new EmployeeRoleEntity { IdRole = 1, RoleName = "Piloto" },
            new EmployeeRoleEntity { IdRole = 2, RoleName = "Copiloto" },
            new EmployeeRoleEntity { IdRole = 3, RoleName = "Auxiliar de vuelo" },
            new EmployeeRoleEntity { IdRole = 4, RoleName = "Agente de aeropuerto" },
            new EmployeeRoleEntity { IdRole = 5, RoleName = "Supervisor de operaciones" }
        );
    }
}
