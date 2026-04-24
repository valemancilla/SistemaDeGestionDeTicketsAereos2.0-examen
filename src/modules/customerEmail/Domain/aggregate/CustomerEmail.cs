// El correo del cliente es un dato de contacto, una persona puede tener varios correos registrados
using SistemaDeGestionDeTicketsAereos.src.modules.customerEmail.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.customerEmail.Domain.aggregate;

// Agregado CustomerEmail: encapsula las reglas de negocio de un correo electrónico de cliente
public class CustomerEmail
{
    // ID del registro de correo (Value Object)
    public CustomerEmailId Id { get; private set; }

    // La dirección de correo electrónico (con validación de formato)
    public CustomerEmailAddress Email { get; private set; }

    // FK a la persona propietaria del correo
    public int IdPerson { get; private set; }

    // Constructor privado: solo se crea a través del método Create
    private CustomerEmail(CustomerEmailId id, CustomerEmailAddress email, int idPerson)
    {
        Id = id;
        Email = email;
        IdPerson = idPerson;
    }

    // Método de fábrica para crear o reconstruir un correo desde la base de datos
    public static CustomerEmail Create(int id, string email, int idPerson)
    {
        // Regla: el email debe estar asociado a una persona válida
        if (idPerson <= 0)
            throw new ArgumentException("IdPerson must be greater than 0.", nameof(idPerson));

        // Regla: el formato del email es validado por su Value Object
        return new CustomerEmail(
            CustomerEmailId.Create(id),
            CustomerEmailAddress.Create(email),
            idPerson
        );
    }

    // Método de fábrica para crear un correo nuevo (ID = 0, la BD lo asigna después)
    public static CustomerEmail CreateNew(string email, int idPerson) => Create(0, email, idPerson);
}
