using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.role.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.user.Infrastructure.Entity;

// Configuración de EF Core para mapear UserEntity a la tabla User
public sealed class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable("User");

        // Clave primaria
        builder.HasKey(x => x.IdUser);

        // El ID se genera automáticamente
        builder.Property(x => x.IdUser)
            .HasColumnName("IdUser")
            .ValueGeneratedOnAdd();

        // Nombre de usuario, obligatorio, máximo 60 caracteres
        builder.Property(x => x.Username)
            .HasColumnName("Username")
            .HasColumnType("varchar(60)")
            .IsRequired();

        // El username debe ser único para que no haya dos usuarios con el mismo nombre
        builder.HasIndex(x => x.Username)
            .IsUnique()
            .HasDatabaseName("UQ_User_Username");

        // Contraseña, obligatoria, hasta 255 caracteres (espacio para hashes)
        builder.Property(x => x.Password)
            .HasColumnName("Password")
            .HasColumnType("varchar(255)")
            .IsRequired();

        // FK al rol del usuario, obligatoria
        builder.Property(x => x.IdUserRole)
            .HasColumnName("IdUserRole")
            .IsRequired();

        // FK a la persona, opcional (un admin puede no tener persona vinculada)
        builder.Property(x => x.IdPerson)
            .HasColumnName("IdPerson");

        // Active por defecto es true cuando se crea el usuario
        builder.Property(x => x.Active)
            .HasColumnName("Active")
            .HasColumnType("tinyint(1)")
            .HasDefaultValue(true)
            .IsRequired();

        // Relación: un usuario tiene un rol asignado
        builder.HasOne(x => x.Role)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.IdUserRole)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación: un usuario puede estar vinculado a una persona (es opcional)
        builder.HasOne(x => x.Person)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.IdPerson)
            .OnDelete(DeleteBehavior.Restrict);

        // Solo administrador inicial; los clientes se crean por registro en el menú de login.
        builder.HasData(
            new UserEntity { IdUser = 1, Username = "admin", Password = "12345678", IdUserRole = 1, Active = true }
        );
    }
}
