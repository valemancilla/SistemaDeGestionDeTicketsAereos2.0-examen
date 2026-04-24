using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SistemaDeGestionDeTicketsAereos.src.modules.documentType.Infrastructure.Entity;

// Configuración de EF Core para la tabla DocumentType
public sealed class DocumentTypeEntityConfiguration : IEntityTypeConfiguration<DocumentTypeEntity>
{
    public void Configure(EntityTypeBuilder<DocumentTypeEntity> builder)
    {
        // Tabla en la BD
        builder.ToTable("DocumentType");

        // Clave primaria
        builder.HasKey(x => x.IdDocumentType);

        // Se genera automáticamente
        builder.Property(x => x.IdDocumentType)
            .HasColumnName("IdDocumentType")
            .ValueGeneratedOnAdd();

        // Nombre del tipo de documento, obligatorio, máximo 50 caracteres
        builder.Property(x => x.Name)
            .HasColumnName("Name")
            .HasColumnType("varchar(50)")
            .IsRequired();

        builder.HasData(
            new DocumentTypeEntity { IdDocumentType = 1, Name = "Cédula de Ciudadanía" },
            new DocumentTypeEntity { IdDocumentType = 2, Name = "Pasaporte" },
            new DocumentTypeEntity { IdDocumentType = 3, Name = "Cédula de Extranjería" },
            new DocumentTypeEntity { IdDocumentType = 4, Name = "Tarjeta de Identidad" }
        );
    }
}
