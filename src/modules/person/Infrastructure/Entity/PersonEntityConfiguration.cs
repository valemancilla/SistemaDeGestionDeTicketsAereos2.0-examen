using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeGestionDeTicketsAereos.src.modules.country.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.documentType.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.gender.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.person.Infrastructure.Entity;

// Configuración de EF Core para mapear PersonEntity a la tabla Person
public sealed class PersonEntityConfiguration : IEntityTypeConfiguration<PersonEntity>
{
    public void Configure(EntityTypeBuilder<PersonEntity> builder)
    {
        builder.ToTable("Person");

        // Clave primaria
        builder.HasKey(x => x.IdPerson);

        // El ID se genera automáticamente
        builder.Property(x => x.IdPerson)
            .HasColumnName("IdPerson")
            .ValueGeneratedOnAdd();

        // FK al tipo de documento, obligatoria
        builder.Property(x => x.IdDocumentType)
            .HasColumnName("IdDocumentType")
            .IsRequired();

        // Número de documento, obligatorio, máximo 30 caracteres, único en toda la tabla
        builder.Property(x => x.DocumentNumber)
            .HasColumnName("DocumentNumber")
            .HasColumnType("varchar(30)")
            .IsRequired();

        builder.HasIndex(x => x.DocumentNumber)
            .IsUnique()
            .HasDatabaseName("UQ_Person_DocumentNumber");

        // Nombre de la persona, obligatorio, máximo 80 caracteres
        builder.Property(x => x.FirstName)
            .HasColumnName("FirstName")
            .HasColumnType("varchar(80)")
            .IsRequired();

        // Apellido de la persona, obligatorio, máximo 80 caracteres
        builder.Property(x => x.LastName)
            .HasColumnName("LastName")
            .HasColumnType("varchar(80)")
            .IsRequired();

        // Fecha de nacimiento, solo fecha sin hora
        builder.Property(x => x.BirthDate)
            .HasColumnName("BirthDate")
            .HasColumnType("date")
            .IsRequired();

        // FK al género, obligatoria
        builder.Property(x => x.IdGender)
            .HasColumnName("IdGender")
            .IsRequired();

        // FK al país, obligatoria
        builder.Property(x => x.IdCountry)
            .HasColumnName("IdCountry")
            .IsRequired();

        // FK a la dirección actual, opcional (puede ser null si no tiene dirección registrada)
        builder.Property(x => x.IdAddress)
            .HasColumnName("IdAddress");

        // Relación: una persona tiene un tipo de documento
        builder.HasOne(x => x.DocumentType)
            .WithMany(x => x.Persons)
            .HasForeignKey(x => x.IdDocumentType)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación: una persona tiene un género
        builder.HasOne(x => x.Gender)
            .WithMany(x => x.Persons)
            .HasForeignKey(x => x.IdGender)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación: una persona pertenece a un país
        builder.HasOne(x => x.Country)
            .WithMany(x => x.Persons)
            .HasForeignKey(x => x.IdCountry)
            .OnDelete(DeleteBehavior.Restrict);

        // FK opcional: apunta a una de las direcciones de la persona como la "dirección actual"
        // WithMany() sin parámetro porque PersonAddress no tiene una colección de vuelta para este caso
        builder.HasOne(x => x.CurrentAddress)
            .WithMany()
            .HasForeignKey(x => x.IdAddress)
            .OnDelete(DeleteBehavior.Restrict);

        // Documentos únicos para tripulación demo (empleados / CrewMember).
        builder.HasData(
            new PersonEntity
            {
                IdPerson = 1,
                IdDocumentType = 1,
                DocumentNumber = "SEED-TRIP-00000001",
                FirstName = "Juan",
                LastName = "Pérez",
                BirthDate = new DateOnly(1985, 3, 10),
                IdGender = 1,
                IdCountry = 1,
                IdAddress = null
            },
            new PersonEntity
            {
                IdPerson = 2,
                IdDocumentType = 1,
                DocumentNumber = "SEED-TRIP-00000002",
                FirstName = "María",
                LastName = "Gómez",
                BirthDate = new DateOnly(1988, 7, 22),
                IdGender = 2,
                IdCountry = 1,
                IdAddress = null
            });
    }
}
