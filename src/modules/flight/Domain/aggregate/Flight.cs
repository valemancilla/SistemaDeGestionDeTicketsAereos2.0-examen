// El vuelo es el evento central del sistema: conecta una ruta, una aeronave, una tripulación y pasajeros
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.aggregate;

// Agregado Flight: encapsula las reglas de negocio de un vuelo programado
public class Flight
{
    // ID del vuelo (Value Object)
    public FlightId Id { get; private set; }

    // Número de vuelo en formato IATA (ej: "AV123", "LA456")
    public FlightNumber Number { get; private set; }

    // Fecha en que opera el vuelo
    public FlightDate Date { get; private set; }

    // Hora de salida del vuelo
    public FlightDepartureTime DepartureTime { get; private set; }

    // Hora de llegada al destino
    public FlightArrivalTime ArrivalTime { get; private set; }

    // Capacidad total de asientos de la aeronave asignada
    public FlightTotalCapacity TotalCapacity { get; private set; }

    // Asientos aún disponibles para reserva — disminuye con cada tiquete vendido
    public FlightAvailableSeats AvailableSeats { get; private set; }

    // FK a la ruta origen-destino que opera este vuelo
    public int IdRoute { get; private set; }

    // FK a la aeronave asignada al vuelo
    public int IdAircraft { get; private set; }

    // FK al estado actual del vuelo (programado, en curso, cancelado, etc.)
    public int IdStatus { get; private set; }

    // FK al grupo de tripulación asignado al vuelo
    public int IdCrew { get; private set; }

    // FK a la tarifa asignada a este vuelo (precio por vuelo). Null = sin tarifa asignada (compatibilidad).
    public int? IdFare { get; private set; }

    // Puerta de embarque publicada con el vuelo (misma que usa el pase al hacer check-in).
    public string BoardingGate { get; private set; } = "A01";

    // Constructor privado: solo se crea a través del método Create
    private Flight(FlightId id, FlightNumber number, FlightDate date,
        FlightDepartureTime departureTime, FlightArrivalTime arrivalTime,
        FlightTotalCapacity totalCapacity, FlightAvailableSeats availableSeats,
        int idRoute, int idAircraft, int idStatus, int idCrew, int? idFare,
        string boardingGate)
    {
        Id = id;
        Number = number;
        Date = date;
        DepartureTime = departureTime;
        ArrivalTime = arrivalTime;
        TotalCapacity = totalCapacity;
        AvailableSeats = availableSeats;
        IdRoute = idRoute;
        IdAircraft = idAircraft;
        IdStatus = idStatus;
        IdCrew = idCrew;
        IdFare = idFare;
        BoardingGate = boardingGate;
    }

    // Método de fábrica para crear o reconstruir un vuelo desde la base de datos
    public static Flight Create(int id, string number, DateOnly date,
        TimeOnly departureTime, TimeOnly arrivalTime,
        int totalCapacity, int availableSeats,
        int idRoute, int idAircraft, int idStatus, int idCrew, int? idFare,
        string? boardingGate = null)
    {
        var gate = string.IsNullOrWhiteSpace(boardingGate) ? "A01" : boardingGate.Trim();
        if (gate.Length > 20)
            throw new ArgumentException("Boarding gate is too long (max 20).", nameof(boardingGate));
        // Regla: el vuelo debe operar en una ruta válida
        if (idRoute <= 0)
            throw new ArgumentException("IdRoute must be greater than 0.", nameof(idRoute));

        // Regla: el vuelo debe tener una aeronave asignada válida
        if (idAircraft <= 0)
            throw new ArgumentException("IdAircraft must be greater than 0.", nameof(idAircraft));

        // Regla: el estado del vuelo debe ser una referencia válida
        if (idStatus <= 0)
            throw new ArgumentException("IdStatus must be greater than 0.", nameof(idStatus));

        // Regla: el vuelo debe tener una tripulación asignada válida
        if (idCrew <= 0)
            throw new ArgumentException("IdCrew must be greater than 0.", nameof(idCrew));

        // Regla: la hora de salida debe ser anterior a la hora de llegada
        if (departureTime >= arrivalTime)
            throw new ArgumentException("Departure time must be before arrival time.", nameof(departureTime));

        // Regla: los asientos disponibles no pueden superar la capacidad total de la aeronave
        if (availableSeats > totalCapacity)
            throw new ArgumentException("Available seats cannot exceed total capacity.", nameof(availableSeats));

        if (idFare is int v && v <= 0)
            throw new ArgumentException("IdFare must be > 0 when provided.", nameof(idFare));

        return new Flight(
            FlightId.Create(id),
            FlightNumber.Create(number),
            FlightDate.Create(date),
            FlightDepartureTime.Create(departureTime),
            FlightArrivalTime.Create(arrivalTime),
            FlightTotalCapacity.Create(totalCapacity),
            FlightAvailableSeats.Create(availableSeats),
            idRoute,
            idAircraft,
            idStatus,
            idCrew,
            idFare,
            gate
        );
    }

    // Método de fábrica para crear un vuelo nuevo (ID = 0, la BD lo asigna después)
    public static Flight CreateNew(string number, DateOnly date, TimeOnly departureTime, TimeOnly arrivalTime, int totalCapacity, int availableSeats, int idRoute, int idAircraft, int idStatus, int idCrew, int? idFare, string? boardingGate = null)
        => Create(0, number, date, departureTime, arrivalTime, totalCapacity, availableSeats, idRoute, idAircraft, idStatus, idCrew, idFare, boardingGate);
}
