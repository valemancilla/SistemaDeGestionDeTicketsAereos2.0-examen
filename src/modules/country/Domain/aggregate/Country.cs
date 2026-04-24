// El país es la base geográfica del sistema, toda ciudad, aerolínea y persona pertenece a uno
using SistemaDeGestionDeTicketsAereos.src.modules.country.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.country.Domain.aggregate;

// Agregado Country: encapsula las reglas de negocio de un país en el sistema
public class Country
{
    // ID del país (Value Object)
    public CountryId Id { get; private set; }

    // Nombre del país (Value Object con validación de formato)
    public CountryName Name { get; private set; }

    // Código ISO de 2 letras del país (ej: CO, US, MX) — Value Object
    public CountryISOCode ISOCode { get; private set; }

    // Constructor privado: solo se crea a través del método Create
    private Country(CountryId id, CountryName name, CountryISOCode isoCode)
    {
        Id = id;
        Name = name;
        ISOCode = isoCode;
    }

    // Método de fábrica para crear o reconstruir un país desde la base de datos
    public static Country Create(int id, string name, string isoCode)
    {
        // Regla: nombre y código ISO son validados por sus respectivos Value Objects
        return new Country(
            CountryId.Create(id),
            CountryName.Create(name),
            CountryISOCode.Create(isoCode)
        );
    }

    // Método de fábrica para crear un país nuevo (ID = 0, la BD lo asigna después)
    public static Country CreateNew(string name, string isoCode)
        => Create(0, name, isoCode);
}
