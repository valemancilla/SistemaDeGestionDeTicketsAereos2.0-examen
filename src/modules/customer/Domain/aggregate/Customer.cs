// El cliente es la persona que compra tiquetes, está vinculado a una persona del sistema
using SistemaDeGestionDeTicketsAereos.src.modules.customer.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.customer.Domain.aggregate;

// Agregado Customer: encapsula las reglas de negocio de un cliente registrado
public class Customer
{
    // ID del cliente (Value Object)
    public CustomerId Id { get; private set; }

    // Fecha en que el cliente se registró en el sistema
    public CustomerRegistrationDate RegistrationDate { get; private set; }

    // FK a la persona asociada — el cliente es una extensión de Person
    public int IdPerson { get; private set; }

    // Indica si el cliente está activo o fue dado de baja
    public bool Active { get; private set; }

    // Constructor privado: solo se crea a través del método Create
    private Customer(CustomerId id, CustomerRegistrationDate registrationDate, int idPerson, bool active)
    {
        Id = id;
        RegistrationDate = registrationDate;
        IdPerson = idPerson;
        Active = active;
    }

    // Método de fábrica para crear o reconstruir un cliente desde la base de datos
    public static Customer Create(int id, DateOnly registrationDate, int idPerson, bool active)
    {
        // Regla: todo cliente debe estar vinculado a una persona válida
        if (idPerson <= 0)
            throw new ArgumentException("IdPerson must be greater than 0.", nameof(idPerson));

        return new Customer(
            CustomerId.Create(id),
            CustomerRegistrationDate.Create(registrationDate),
            idPerson,
            active
        );
    }

    // Método de fábrica para crear un cliente nuevo (ID = 0, la BD lo asigna después)
    public static Customer CreateNew(DateOnly registrationDate, int idPerson, bool active) => Create(0, registrationDate, idPerson, active);
}
