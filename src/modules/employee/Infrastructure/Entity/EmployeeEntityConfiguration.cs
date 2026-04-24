using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.employee.Infrastructure.Entity;

// Configuración de EF Core para mapear EmployeeEntity a la tabla Employee
public sealed class EmployeeEntityConfiguration : IEntityTypeConfiguration<EmployeeEntity>
{
    public void Configure(EntityTypeBuilder<EmployeeEntity> builder)
    {
        builder.ToTable("Employee");

        // Clave primaria
        builder.HasKey(x => x.IdEmployee);

        // El ID se genera automáticamente
        builder.Property(x => x.IdEmployee)
            .HasColumnName("IdEmployee")
            .ValueGeneratedOnAdd();

        // FK a la persona, obligatoria
        builder.Property(x => x.IdPerson)
            .HasColumnName("IdPerson")
            .IsRequired();

        // FK a la aerolínea, obligatoria
        builder.Property(x => x.IdAirline)
            .HasColumnName("IdAirline")
            .IsRequired();

        // FK al rol del empleado, obligatoria
        builder.Property(x => x.IdRole)
            .HasColumnName("IdRole")
            .IsRequired();

        // Relación: un empleado tiene sus datos personales en la tabla Person
        builder.HasOne(x => x.Person)
            .WithMany(x => x.Employees)
            .HasForeignKey(x => x.IdPerson)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación: un empleado trabaja para una aerolínea específica
        builder.HasOne(x => x.Airline)
            .WithMany(x => x.Employees)
            .HasForeignKey(x => x.IdAirline)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación: un empleado tiene un rol definido dentro de la aerolínea
        builder.HasOne(x => x.Role)
            .WithMany(x => x.Employees)
            .HasForeignKey(x => x.IdRole)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new EmployeeEntity { IdEmployee = 1, IdPerson = 1, IdAirline = 1, IdRole = 1 },
            new EmployeeEntity { IdEmployee = 2, IdPerson = 2, IdAirline = 1, IdRole = 2 });
    }
}
