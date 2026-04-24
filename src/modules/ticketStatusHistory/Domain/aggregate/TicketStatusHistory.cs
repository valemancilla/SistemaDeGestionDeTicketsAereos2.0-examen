// El historial de estados del tiquete registra cada cambio (emitido → usado → cancelado, etc.)
using SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Domain.aggregate;

// Agregado TicketStatusHistory: encapsula las reglas de un registro de cambio de estado de tiquete
public class TicketStatusHistory
{
    // ID del registro en el historial (Value Object)
    public TicketStatusHistoryId Id { get; private set; }

    // Fecha y hora exacta en que cambió el estado del tiquete
    public TicketStatusHistoryChangeDate ChangeDate { get; private set; }

    // Observación opcional que explica el motivo del cambio
    public TicketStatusHistoryObservation Observation { get; private set; }

    // FK al tiquete cuyo estado cambió
    public int IdTicket { get; private set; }

    // FK al nuevo estado que adquirió el tiquete
    public int IdStatus { get; private set; }

    // FK al usuario del sistema que registró el cambio
    public int IdUser { get; private set; }

    // Constructor privado: solo se crea a través del método Create
    private TicketStatusHistory(TicketStatusHistoryId id, TicketStatusHistoryChangeDate changeDate,
        TicketStatusHistoryObservation observation, int idTicket, int idStatus, int idUser)
    {
        Id = id;
        ChangeDate = changeDate;
        Observation = observation;
        IdTicket = idTicket;
        IdStatus = idStatus;
        IdUser = idUser;
    }

    // Método de fábrica para crear o reconstruir un registro del historial desde la base de datos
    public static TicketStatusHistory Create(int id, DateTime changeDate,
        string? observation, int idTicket, int idStatus, int idUser)
    {
        // Regla: el registro debe estar asociado a un tiquete válido
        if (idTicket <= 0)
            throw new ArgumentException("IdTicket must be greater than 0.", nameof(idTicket));

        // Regla: el nuevo estado debe ser una referencia válida
        if (idStatus <= 0)
            throw new ArgumentException("IdStatus must be greater than 0.", nameof(idStatus));

        // Regla: el cambio de estado debe ser registrado por un usuario válido
        if (idUser <= 0)
            throw new ArgumentException("IdUser must be greater than 0.", nameof(idUser));

        // Regla: la fecha del cambio no puede ser futura
        if (changeDate > DateTime.Now)
            throw new ArgumentException("Change date cannot be in the future.", nameof(changeDate));

        return new TicketStatusHistory(
            TicketStatusHistoryId.Create(id),
            TicketStatusHistoryChangeDate.Create(changeDate),
            TicketStatusHistoryObservation.Create(observation),
            idTicket,
            idStatus,
            idUser
        );
    }

    // Método de fábrica para crear un registro nuevo (ID = 0, la BD lo asigna después)
    public static TicketStatusHistory CreateNew(DateTime changeDate, string? observation, int idTicket, int idStatus, int idUser)
        => Create(0, changeDate, observation, idTicket, idStatus, idUser);
}
