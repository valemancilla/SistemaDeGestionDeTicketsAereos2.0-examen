using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SistemaDeGestionDeTicketsAereos.src.modules.country.Infrastructure.Entity;

// Configuración de EF Core para la tabla Country
public sealed class CountryEntityConfiguration : IEntityTypeConfiguration<CountryEntity>
{
    public void Configure(EntityTypeBuilder<CountryEntity> builder)
    {
        // Nombre de la tabla en la base de datos
        builder.ToTable("Country");

        // Clave primaria
        builder.HasKey(x => x.IdCountry);

        // El ID se genera automáticamente en la BD
        builder.Property(x => x.IdCountry)
            .HasColumnName("IdCountry")
            .ValueGeneratedOnAdd();

        // Nombre del país, obligatorio, máximo 100 caracteres
        builder.Property(x => x.Name)
            .HasColumnName("Name")
            .HasColumnType("varchar(100)")
            .IsRequired();

        // Código ISO de exactamente 2 caracteres (CO, US, etc.)
        builder.Property(x => x.ISOCode)
            .HasColumnName("ISOCode")
            .HasColumnType("char(2)")
            .IsRequired();

        // El código ISO no puede repetirse entre países
        builder.HasIndex(x => x.ISOCode)
            .IsUnique()
            .HasDatabaseName("UQ_Country_ISOCode");

        builder.HasData(
            new CountryEntity { IdCountry =  1, Name = "Colombia",       ISOCode = "CO" },
            new CountryEntity { IdCountry =  2, Name = "Venezuela",      ISOCode = "VE" },
            new CountryEntity { IdCountry =  3, Name = "Ecuador",        ISOCode = "EC" },
            new CountryEntity { IdCountry =  4, Name = "Perú",           ISOCode = "PE" },
            new CountryEntity { IdCountry =  5, Name = "Brasil",         ISOCode = "BR" },
            new CountryEntity { IdCountry =  6, Name = "Argentina",      ISOCode = "AR" },
            new CountryEntity { IdCountry =  7, Name = "Chile",          ISOCode = "CL" },
            new CountryEntity { IdCountry =  8, Name = "México",         ISOCode = "MX" },
            new CountryEntity { IdCountry =  9, Name = "Panamá",         ISOCode = "PA" },
            new CountryEntity { IdCountry = 10, Name = "España",         ISOCode = "ES" },
            new CountryEntity { IdCountry = 11, Name = "Estados Unidos", ISOCode = "US" }
        );
    }
}
