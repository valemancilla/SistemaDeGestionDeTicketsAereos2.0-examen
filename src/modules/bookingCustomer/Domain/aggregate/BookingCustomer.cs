// Un BookingCustomer representa la asignación de un pasajero a una reserva con su asiento específico
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Domain.aggregate;

// Agregado BookingCustomer: vincula a una persona con una reserva y un asiento concreto
public class BookingCustomer
{
    // ID del registro de asociación (Value Object)
    public BookingCustomerId Id { get; private set; }

    // Fecha en que el pasajero fue asociado a la reserva
    public BookingCustomerAssociationDate AssociationDate { get; private set; }

    // FK a la reserva a la que pertenece este pasajero
    public int IdBooking { get; private set; }

    // FK al usuario del sistema que registró la asociación
    public int IdUser { get; private set; }

    // FK a la persona (pasajero) que viajará en este asiento
    public int IdPerson { get; private set; }

    // FK al asiento asignado dentro de la aeronave
    public int IdSeat { get; private set; }

    // Indica si este pasajero es el titular principal de la reserva
    public bool IsPrimary { get; private set; }

    // Estado explícito del pasajero dentro de la reserva: listo para abordar tras check-in (Examen 3)
    public bool IsReadyToBoard { get; private set; }

    // Constructor privado: solo se crea a través del método Create
    private BookingCustomer(BookingCustomerId id, BookingCustomerAssociationDate associationDate,
        int idBooking, int idUser, int idPerson, int idSeat, bool isPrimary, bool isReadyToBoard)
    {
        Id = id;
        AssociationDate = associationDate;
        IdBooking = idBooking;
        IdUser = idUser;
        IdPerson = idPerson;
        IdSeat = idSeat;
        IsPrimary = isPrimary;
        IsReadyToBoard = isReadyToBoard;
    }

    // Método de fábrica para crear o reconstruir un BookingCustomer desde la base de datos
    public static BookingCustomer Create(int id, DateTime associationDate,
        int idBooking, int idUser, int idPerson, int idSeat, bool isPrimary)
    {
        return Create(id, associationDate, idBooking, idUser, idPerson, idSeat, isPrimary, isReadyToBoard: false);
    }

    public static BookingCustomer Create(int id, DateTime associationDate,
        int idBooking, int idUser, int idPerson, int idSeat, bool isPrimary, bool isReadyToBoard)
    {
        // Regla: el pasajero debe estar asociado a una reserva válida
        if (idBooking <= 0)
            throw new ArgumentException("IdBooking must be greater than 0.", nameof(idBooking));

        // Regla: la asociación debe ser realizada por un usuario válido del sistema
        if (idUser <= 0)
            throw new ArgumentException("IdUser must be greater than 0.", nameof(idUser));

        // Regla: el pasajero debe ser una persona válida registrada en el sistema
        if (idPerson <= 0)
            throw new ArgumentException("IdPerson must be greater than 0.", nameof(idPerson));

        // Regla: el pasajero debe tener un asiento asignado válido
        if (idSeat <= 0)
            throw new ArgumentException("IdSeat must be greater than 0.", nameof(idSeat));

        return new BookingCustomer(
            BookingCustomerId.Create(id),
            BookingCustomerAssociationDate.Create(associationDate),
            idBooking,
            idUser,
            idPerson,
            idSeat,
            isPrimary,
            isReadyToBoard
        );
    }

    // Método de fábrica para crear un registro nuevo (ID = 0, la BD lo asigna después)
    public static BookingCustomer CreateNew(DateTime associationDate, int idBooking, int idUser, int idPerson, int idSeat, bool isPrimary)
        => Create(0, associationDate, idBooking, idUser, idPerson, idSeat, isPrimary, isReadyToBoard: false);

    public static BookingCustomer CreateNew(DateTime associationDate, int idBooking, int idUser, int idPerson, int idSeat, bool isPrimary, bool isReadyToBoard)
        => Create(0, associationDate, idBooking, idUser, idPerson, idSeat, isPrimary, isReadyToBoard);
}
