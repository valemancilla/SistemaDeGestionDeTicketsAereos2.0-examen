// La dirección de la persona es su ubicación física — una persona puede tener varias, una activa a la vez
using SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Domain.aggregate;

// Agregado PersonAddress: encapsula las reglas de negocio de una dirección física de persona
public class PersonAddress
{
    // ID de la dirección (Value Object)
    public PersonAddressId Id { get; private set; }

    // Dirección completa de vía y placa (ej: "Calle 31 # 21-56")
    public PersonAddressStreet Street { get; private set; }

    // Interior, apartamento, unidad o número de casa (ej: "402", "21-256")
    public PersonAddressNumber Number { get; private set; }

    // Barrio, sector, conjunto u otra referencia de ubicación
    public PersonAddressNeighborhood Neighborhood { get; private set; }

    // Casa o apartamento
    public PersonAddressDwellingType DwellingType { get; private set; }

    // Código postal — es opcional, no todos los países lo usan igual
    public PersonAddressZipCode ZipCode { get; private set; }

    // FK a la persona propietaria de esta dirección
    public int IdPerson { get; private set; }

    // FK a la ciudad donde se ubica la dirección
    public int IdCity { get; private set; }

    // Indica si es la dirección actual activa — permite tener historial de direcciones
    public bool Active { get; private set; }

    // Constructor privado: solo se crea a través del método Create
    private PersonAddress(PersonAddressId id, PersonAddressStreet street, PersonAddressNumber number,
        PersonAddressNeighborhood neighborhood, PersonAddressDwellingType dwellingType,
        PersonAddressZipCode zipCode, int idPerson, int idCity, bool active)
    {
        Id = id;
        Street = street;
        Number = number;
        Neighborhood = neighborhood;
        DwellingType = dwellingType;
        ZipCode = zipCode;
        IdPerson = idPerson;
        IdCity = idCity;
        Active = active;
    }

    // Método de fábrica para crear o reconstruir una dirección desde la base de datos
    public static PersonAddress Create(int id, string street, string number, string neighborhood, string dwellingType,
        string? zipCode, int idPerson, int idCity, bool active)
    {
        // Regla: la dirección debe estar asociada a una persona válida
        if (idPerson <= 0)
            throw new ArgumentException("IdPerson must be greater than 0.", nameof(idPerson));

        // Regla: la dirección debe pertenecer a una ciudad válida
        if (idCity <= 0)
            throw new ArgumentException("IdCity must be greater than 0.", nameof(idCity));

        // Regla: calle, número y código postal son validados por sus Value Objects
        return new PersonAddress(
            PersonAddressId.Create(id),
            PersonAddressStreet.Create(street),
            PersonAddressNumber.Create(number),
            PersonAddressNeighborhood.Create(neighborhood),
            PersonAddressDwellingType.Create(dwellingType),
            PersonAddressZipCode.Create(zipCode),
            idPerson,
            idCity,
            active
        );
    }

    // Método de fábrica para crear una dirección nueva (ID = 0, la BD lo asigna después)
    public static PersonAddress CreateNew(string street, string number, string neighborhood, string dwellingType, string? zipCode, int idPerson, int idCity, bool active)
        => Create(0, street, number, neighborhood, dwellingType, zipCode, idPerson, idCity, active);
}
