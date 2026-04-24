// El historial de estados del vuelo registra cada cambio (programado → en curso → completado, etc.)
using SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Domain.aggregate;

// Agregado FlightStatusHistory: encapsula las reglas de un registro de cambio de estado de vuelo
public class FlightStatusHistory
{
    // ID del registro en el historial (Value Object)
    public FlightStatusHistoryId Id { get; private set; }

    // Fecha y hora exacta en que se realizó el cambio de estado
    public FlightStatusHistoryChangeDate ChangeDate { get; private set; }

    // Observación opcional que explica el motivo del cambio (ej: "Cancelado por mal tiempo")
    public FlightStatusHistoryObservation Observation { get; private set; }

    // FK al vuelo cuyo estado cambió
    public int IdFlight { get; private set; }

    // FK al nuevo estado que adquirió el vuelo
    public int IdStatus { get; private set; }

    // FK al usuario del sistema que registró el cambio
    public int IdUser { get; private set; }

    // Constructor privado: solo se crea a través del método Create
    private FlightStatusHistory(FlightStatusHistoryId id, FlightStatusHistoryChangeDate changeDate,
        FlightStatusHistoryObservation observation, int idFlight, int idStatus, int idUser)
    {
        Id = id;
        ChangeDate = changeDate;
        Observation = observation;
        IdFlight = idFlight;
        IdStatus = idStatus;
        IdUser = idUser;
    }

    // Método de fábrica para crear o reconstruir un registro del historial desde la base de datos
    public static FlightStatusHistory Create(int id, DateTime changeDate,
        string? observation, int idFlight, int idStatus, int idUser)
    {
        // Regla: el registro debe estar asociado a un vuelo válido
        if (idFlight <= 0)
            throw new ArgumentException("IdFlight must be greater than 0.", nameof(idFlight));

        // Regla: el nuevo estado debe ser una referencia válida
        if (idStatus <= 0)
            throw new ArgumentException("IdStatus must be greater than 0.", nameof(idStatus));

        // Regla: el cambio de estado debe ser realizado por un usuario válido
        if (idUser <= 0)
            throw new ArgumentException("IdUser must be greater than 0.", nameof(idUser));

        // Regla: la fecha del cambio de estado no puede ser futura
        if (changeDate > DateTime.Now)
            throw new ArgumentException("Change date cannot be in the future.", nameof(changeDate));

        return new FlightStatusHistory(
            FlightStatusHistoryId.Create(id),
            FlightStatusHistoryChangeDate.Create(changeDate),
            FlightStatusHistoryObservation.Create(observation),
            idFlight,
            idStatus,
            idUser
        );
    }

    // Método de fábrica para crear un registro nuevo (ID = 0, la BD lo asigna después)
    public static FlightStatusHistory CreateNew(DateTime changeDate, string? observation, int idFlight, int idStatus, int idUser)
        => Create(0, changeDate, observation, idFlight, idStatus, idUser);
}
