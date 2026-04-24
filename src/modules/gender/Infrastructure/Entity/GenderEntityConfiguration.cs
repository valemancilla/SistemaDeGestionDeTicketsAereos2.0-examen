using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SistemaDeGestionDeTicketsAereos.src.modules.gender.Infrastructure.Entity;

// Esta clase le dice a EF Core cómo mapear GenderEntity a la tabla real de MySQL
// Implementa IEntityTypeConfiguration para que AppDbContext la detecte automáticamente
public sealed class GenderEntityConfiguration : IEntityTypeConfiguration<GenderEntity>
{
    public void Configure(EntityTypeBuilder<GenderEntity> builder)
    {
        // Le decimos que esta entidad corresponde a la tabla "Gender" en la BD
        builder.ToTable("Gender");

        // Definimos la clave primaria
        builder.HasKey(x => x.IdGender);

        // IdGender se genera automáticamente (AUTO_INCREMENT en MySQL)
        builder.Property(x => x.IdGender)
            .HasColumnName("IdGender")
            .ValueGeneratedOnAdd();

        // Description es obligatorio y tiene máximo 50 caracteres
        builder.Property(x => x.Description)
            .HasColumnName("Description")
            .HasColumnType("varchar(50)")
            .IsRequired();

        builder.HasData(
            new GenderEntity { IdGender = 1, Description = "Masculino" },
            new GenderEntity { IdGender = 2, Description = "Femenino" },
            new GenderEntity { IdGender = 3, Description = "No binario" },
            new GenderEntity { IdGender = 4, Description = "Prefiero no decir" }
        );
    }
}
