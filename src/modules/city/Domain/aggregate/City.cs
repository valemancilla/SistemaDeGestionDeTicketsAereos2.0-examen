// La ciudad pertenece a un país y es el lugar donde se ubican aeropuertos y personas
using SistemaDeGestionDeTicketsAereos.src.modules.city.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.city.Domain.aggregate;

// Agregado City: encapsula las reglas de negocio de una ciudad en el sistema
public class City
{
    // ID de la ciudad (Value Object)
    public CityId Id { get; private set; }

    // Nombre de la ciudad (Value Object con validación)
    public CityName Name { get; private set; }

    // ID del país al que pertenece esta ciudad
    public int IdCountry { get; private set; }

    // Constructor privado: solo se crea a través del método Create
    private City(CityId id, CityName name, int idCountry)
    {
        Id = id;
        Name = name;
        IdCountry = idCountry;
    }

    // Método de fábrica para crear o reconstruir una ciudad desde la base de datos
    public static City Create(int id, string name, int idCountry)
    {
        // Regla: toda ciudad debe estar asociada a un país válido
        if (idCountry <= 0)
            throw new ArgumentException("IdCountry must be greater than 0.", nameof(idCountry));

        return new City(
            CityId.Create(id),
            CityName.Create(name),
            idCountry
        );
    }

    // Método de fábrica para crear una ciudad nueva (ID = 0, la BD lo asigna después)
    public static City CreateNew(string name, int idCountry) => Create(0, name, idCountry);
}
