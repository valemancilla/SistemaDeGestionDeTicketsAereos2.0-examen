// La persona es la entidad base del sistema — tanto clientes como empleados son personas
using SistemaDeGestionDeTicketsAereos.src.modules.person.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.person.Domain.aggregate;

// Agregado Person: encapsula las reglas de negocio de una persona natural en el sistema
public class Person
{
    // ID de la persona (Value Object)
    public PersonId Id { get; private set; }

    // Nombre(s) de la persona
    public PersonFirstName FirstName { get; private set; }

    // Apellido(s) de la persona
    public PersonLastName LastName { get; private set; }

    // Fecha de nacimiento — no puede ser futura ni tener más de 120 años
    public PersonBirthDate BirthDate { get; private set; }

    // Número del documento de identidad (cédula, pasaporte, etc.)
    public PersonDocumentNumber DocumentNumber { get; private set; }

    // FK al tipo de documento que presenta la persona
    public int IdDocumentType { get; private set; }

    // FK al género de la persona
    public int IdGender { get; private set; }

    // FK al país de nacionalidad de la persona
    public int IdCountry { get; private set; }

    // FK a la dirección actual — es opcional porque puede no tener dirección registrada aún
    public int? IdAddress { get; private set; }

    // Constructor privado: solo se crea a través del método Create
    private Person(PersonId id, PersonFirstName firstName, PersonLastName lastName,
        PersonBirthDate birthDate, PersonDocumentNumber documentNumber,
        int idDocumentType, int idGender, int idCountry, int? idAddress)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate;
        DocumentNumber = documentNumber;
        IdDocumentType = idDocumentType;
        IdGender = idGender;
        IdCountry = idCountry;
        IdAddress = idAddress;
    }

    // Método de fábrica para crear o reconstruir una persona desde la base de datos
    public static Person Create(int id, string firstName, string lastName,
        DateOnly birthDate, string documentNumber,
        int idDocumentType, int idGender, int idCountry, int? idAddress)
    {
        // Regla: el tipo de documento debe ser una referencia válida
        if (idDocumentType <= 0)
            throw new ArgumentException("IdDocumentType must be greater than 0.", nameof(idDocumentType));

        // Regla: el género debe ser una referencia válida
        if (idGender <= 0)
            throw new ArgumentException("IdGender must be greater than 0.", nameof(idGender));

        // Regla: el país de nacionalidad debe ser una referencia válida
        if (idCountry <= 0)
            throw new ArgumentException("IdCountry must be greater than 0.", nameof(idCountry));

        // Regla: si se proporciona dirección, debe ser una referencia válida
        if (idAddress.HasValue && idAddress.Value <= 0)
            throw new ArgumentException("IdAddress must be greater than 0.", nameof(idAddress));

        // Regla: la fecha de nacimiento no puede ser futura
        if (birthDate > DateOnly.FromDateTime(DateTime.Today))
            throw new ArgumentException("Birth date cannot be in the future.", nameof(birthDate));

        return new Person(
            PersonId.Create(id),
            PersonFirstName.Create(firstName),
            PersonLastName.Create(lastName),
            PersonBirthDate.Create(birthDate),
            PersonDocumentNumber.Create(documentNumber),
            idDocumentType,
            idGender,
            idCountry,
            idAddress
        );
    }

    // Método de fábrica para crear una persona nueva (ID = 0, la BD lo asigna después)
    public static Person CreateNew(string firstName, string lastName, DateOnly birthDate, string documentNumber, int idDocumentType, int idGender, int idCountry, int? idAddress)
        => Create(0, firstName, lastName, birthDate, documentNumber, idDocumentType, idGender, idCountry, idAddress);
}
