using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaDeGestionDeTicketsAereos.src.modules.country.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.city.Infrastructure.Entity;

// Configuración de EF Core para mapear CityEntity a la tabla City
public sealed class CityEntityConfiguration : IEntityTypeConfiguration<CityEntity>
{
    public void Configure(EntityTypeBuilder<CityEntity> builder)
    {
        builder.ToTable("City");

        // Clave primaria de la tabla
        builder.HasKey(x => x.IdCity);

        // El ID lo genera la BD automáticamente
        builder.Property(x => x.IdCity)
            .HasColumnName("IdCity")
            .ValueGeneratedOnAdd();

        // Nombre de la ciudad, obligatorio, máximo 100 caracteres
        builder.Property(x => x.Name)
            .HasColumnName("Name")
            .HasColumnType("varchar(100)")
            .IsRequired();

        // FK al país, es obligatoria
        builder.Property(x => x.IdCountry)
            .HasColumnName("IdCountry")
            .IsRequired();

        // Relación: una ciudad pertenece a un país, un país tiene muchas ciudades
        // Restrict: no se puede borrar un país si tiene ciudades registradas
        builder.HasOne(x => x.Country)
            .WithMany(x => x.Cities)
            .HasForeignKey(x => x.IdCountry)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new CityEntity { IdCity = 1, Name = "Bogotá", IdCountry = 1 },
            new CityEntity { IdCity = 2, Name = "Medellín", IdCountry = 1 },
            new CityEntity { IdCity = 3, Name = "Cali", IdCountry = 1 },
            new CityEntity { IdCity = 4, Name = "Cartagena", IdCountry = 1 },
            new CityEntity { IdCity = 5, Name = "Barranquilla", IdCountry = 1 },
            new CityEntity { IdCity = 6, Name = "Caracas", IdCountry = 2 },
            new CityEntity { IdCity = 7, Name = "Quito", IdCountry = 3 },
            new CityEntity { IdCity = 8, Name = "Guayaquil", IdCountry = 3 },
            new CityEntity { IdCity = 9, Name = "Lima", IdCountry = 4 },
            new CityEntity { IdCity = 10, Name = "São Paulo", IdCountry = 5 },
            new CityEntity { IdCity = 11, Name = "Río de Janeiro", IdCountry = 5 },
            new CityEntity { IdCity = 12, Name = "Buenos Aires", IdCountry = 6 },
            new CityEntity { IdCity = 13, Name = "Santiago", IdCountry = 7 },
            new CityEntity { IdCity = 14, Name = "Ciudad de México", IdCountry = 8 },
            new CityEntity { IdCity = 15, Name = "Cancún", IdCountry = 8 },
            new CityEntity { IdCity = 16, Name = "Ciudad de Panamá", IdCountry = 9 },
            new CityEntity { IdCity = 17, Name = "Madrid", IdCountry = 10 },
            new CityEntity { IdCity = 18, Name = "Barcelona", IdCountry = 10 },
            new CityEntity { IdCity = 19, Name = "Miami", IdCountry = 11 },
            new CityEntity { IdCity = 20, Name = "Nueva York", IdCountry = 11 }
        );
    }
}
