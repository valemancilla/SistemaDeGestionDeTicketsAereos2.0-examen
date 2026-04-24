using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeGestionDeTicketsAereos.src.modules.crew.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.employee.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.employeeRole.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Infrastructure.Entity;

// Configuración de EF Core para mapear CrewMemberEntity a la tabla CrewMember
public sealed class CrewMemberEntityConfiguration : IEntityTypeConfiguration<CrewMemberEntity>
{
    public void Configure(EntityTypeBuilder<CrewMemberEntity> builder)
    {
        builder.ToTable("CrewMember");

        // Clave primaria
        builder.HasKey(x => x.IdCrewMember);

        // El ID se genera automáticamente
        builder.Property(x => x.IdCrewMember)
            .HasColumnName("IdCrewMember")
            .ValueGeneratedOnAdd();

        // FK a la tripulación, obligatoria
        builder.Property(x => x.IdCrew)
            .HasColumnName("IdCrew")
            .IsRequired();

        // FK al empleado, obligatoria
        builder.Property(x => x.IdEmployee)
            .HasColumnName("IdEmployee")
            .IsRequired();

        // FK al rol dentro de la tripulación, obligatoria
        builder.Property(x => x.IdRole)
            .HasColumnName("IdRole")
            .IsRequired();

        // Relación: un miembro pertenece a una tripulación, una tripulación puede tener varios miembros
        builder.HasOne(x => x.Crew)
            .WithMany(x => x.CrewMembers)
            .HasForeignKey(x => x.IdCrew)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación: un miembro de tripulación es un empleado específico
        builder.HasOne(x => x.Employee)
            .WithMany(x => x.CrewMembers)
            .HasForeignKey(x => x.IdEmployee)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación: un miembro tiene un rol específico dentro de la tripulación
        builder.HasOne(x => x.EmployeeRole)
            .WithMany(x => x.CrewMembers)
            .HasForeignKey(x => x.IdRole)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new CrewMemberEntity { IdCrewMember = 1, IdCrew = 1, IdEmployee = 1, IdRole = 1 },
            new CrewMemberEntity { IdCrewMember = 2, IdCrew = 1, IdEmployee = 2, IdRole = 2 });
    }
}
