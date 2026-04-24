// El historial de estados de reserva registra cada transición (pendiente → confirmada → cancelada, etc.)
using SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Domain.aggregate;

// Agregado BookingStatusHistory: encapsula las reglas de un registro de cambio de estado de reserva
public class BookingStatusHistory
{
    // ID del registro en el historial (Value Object)
    public BookingStatusHistoryId Id { get; private set; }

    // Fecha y hora exacta en que cambió el estado de la reserva
    public BookingStatusHistoryChangeDate ChangeDate { get; private set; }

    // Observación opcional que explica el motivo del cambio
    public BookingStatusHistoryObservation Observation { get; private set; }

    // FK a la reserva cuyo estado cambió
    public int IdBooking { get; private set; }

    // FK al nuevo estado que adquirió la reserva
    public int IdStatus { get; private set; }

    // FK al usuario del sistema que registró el cambio
    public int IdUser { get; private set; }

    // Constructor privado: solo se crea a través del método Create
    private BookingStatusHistory(BookingStatusHistoryId id, BookingStatusHistoryChangeDate changeDate,
        BookingStatusHistoryObservation observation, int idBooking, int idStatus, int idUser)
    {
        Id = id;
        ChangeDate = changeDate;
        Observation = observation;
        IdBooking = idBooking;
        IdStatus = idStatus;
        IdUser = idUser;
    }

    // Método de fábrica para crear o reconstruir un registro del historial desde la base de datos
    public static BookingStatusHistory Create(int id, DateTime changeDate,
        string? observation, int idBooking, int idStatus, int idUser)
    {
        // Regla: el registro debe estar asociado a una reserva válida
        if (idBooking <= 0)
            throw new ArgumentException("IdBooking must be greater than 0.", nameof(idBooking));

        // Regla: el nuevo estado debe ser una referencia válida
        if (idStatus <= 0)
            throw new ArgumentException("IdStatus must be greater than 0.", nameof(idStatus));

        // Regla: el cambio de estado debe ser registrado por un usuario válido
        if (idUser <= 0)
            throw new ArgumentException("IdUser must be greater than 0.", nameof(idUser));

        // Regla: la fecha del cambio no puede ser futura
        if (changeDate > DateTime.Now)
            throw new ArgumentException("Change date cannot be in the future.", nameof(changeDate));

        return new BookingStatusHistory(
            BookingStatusHistoryId.Create(id),
            BookingStatusHistoryChangeDate.Create(changeDate),
            BookingStatusHistoryObservation.Create(observation),
            idBooking,
            idStatus,
            idUser
        );
    }

    // Método de fábrica para crear un registro nuevo (ID = 0, la BD lo asigna después)
    public static BookingStatusHistory CreateNew(DateTime changeDate, string? observation, int idBooking, int idStatus, int idUser)
        => Create(0, changeDate, observation, idBooking, idStatus, idUser);
}
