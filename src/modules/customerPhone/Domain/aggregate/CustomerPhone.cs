// El teléfono del cliente es un dato de contacto, una persona puede tener varios números registrados
using SistemaDeGestionDeTicketsAereos.src.modules.customerPhone.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.customerPhone.Domain.aggregate;

// Agregado CustomerPhone: encapsula las reglas de negocio de un número de teléfono de cliente
public class CustomerPhone
{
    // ID del registro de teléfono (Value Object)
    public CustomerPhoneId Id { get; private set; }

    // El número de teléfono (con validación de formato internacional)
    public CustomerPhoneNumber Phone { get; private set; }

    // FK a la persona propietaria del número
    public int IdPerson { get; private set; }

    // Constructor privado: solo se crea a través del método Create
    private CustomerPhone(CustomerPhoneId id, CustomerPhoneNumber phone, int idPerson)
    {
        Id = id;
        Phone = phone;
        IdPerson = idPerson;
    }

    // Método de fábrica para crear o reconstruir un teléfono desde la base de datos
    public static CustomerPhone Create(int id, string phone, int idPerson)
    {
        // Regla: el teléfono debe estar asociado a una persona válida
        if (idPerson <= 0)
            throw new ArgumentException("IdPerson must be greater than 0.", nameof(idPerson));

        // Regla: el formato del teléfono es validado por su Value Object
        return new CustomerPhone(
            CustomerPhoneId.Create(id),
            CustomerPhoneNumber.Create(phone),
            idPerson
        );
    }

    // Método de fábrica para crear un teléfono nuevo (ID = 0, la BD lo asigna después)
    public static CustomerPhone CreateNew(string phone, int idPerson) => Create(0, phone, idPerson);
}
