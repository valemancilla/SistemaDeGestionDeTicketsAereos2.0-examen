// La reserva es el núcleo del sistema: agrupa vuelo, asientos, estado y las observaciones del cliente
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.aggregate;

// Agregado Booking: encapsula las reglas de negocio de una reserva aérea
public class Booking
{
    // ID de la reserva (Value Object)
    public BookingId Id { get; private set; }

    // Código único que se le entrega al cliente para identificar su reserva
    public BookingCode Code { get; private set; }

    // Fecha del vuelo reservado
    public BookingFlightDate FlightDate { get; private set; }

    // Fecha en que se creó la reserva en el sistema
    public BookingCreationDate CreationDate { get; private set; }

    // Cantidad de asientos reservados (máximo 9 por reserva)
    public BookingSeatCount SeatCount { get; private set; }

    // Observaciones opcionales del cliente sobre la reserva
    public BookingObservations Observations { get; private set; }

    // Referencia al vuelo asociado a esta reserva
    public int IdFlight { get; private set; }

    // Estado actual de la reserva (confirmada, pendiente, cancelada, etc.)
    public int IdStatus { get; private set; }

    // Constructor privado: solo se crea desde el método Create para mantener el control de las reglas
    private Booking(BookingId id, BookingCode code, BookingFlightDate flightDate,
        BookingCreationDate creationDate, BookingSeatCount seatCount, BookingObservations observations,
        int idFlight, int idStatus)
    {
        Id = id;
        Code = code;
        FlightDate = flightDate;
        CreationDate = creationDate;
        SeatCount = seatCount;
        Observations = observations;
        IdFlight = idFlight;
        IdStatus = idStatus;
    }

    // Método de fábrica: crea o reconstruye una reserva validando todas las reglas de negocio
    public static Booking Create(int id, string code, DateTime flightDate,
        DateOnly creationDate, int seatCount, string? observations,
        int idFlight, int idStatus)
    {
        // Regla: la reserva debe estar asociada a un vuelo válido
        if (idFlight <= 0)
            throw new ArgumentException("IdFlight must be greater than 0.", nameof(idFlight));

        // Regla: el estado de la reserva debe ser una referencia válida
        if (idStatus <= 0)
            throw new ArgumentException("IdStatus must be greater than 0.", nameof(idStatus));

        // Regla: no se puede crear una reserva para un vuelo con fecha pasada
        if (flightDate < DateTime.Now)
            throw new ArgumentException("Flight date cannot be in the past.", nameof(flightDate));

        // Regla: la fecha de creación de la reserva no puede ser futura
        if (creationDate > DateOnly.FromDateTime(DateTime.Today))
            throw new ArgumentException("Creation date cannot be in the future.", nameof(creationDate));

        // Regla: código, cantidad de asientos y observaciones son validados por sus Value Objects
        return new Booking(
            BookingId.Create(id),
            BookingCode.Create(code),
            BookingFlightDate.Create(flightDate),
            BookingCreationDate.Create(creationDate),
            BookingSeatCount.Create(seatCount),
            BookingObservations.Create(observations),
            idFlight,
            idStatus
        );
    }

    // Método para crear una reserva nueva con ID 0 (la base de datos asigna el ID después)
    public static Booking CreateNew(string code, DateTime flightDate, DateOnly creationDate, int seatCount, string? observations, int idFlight, int idStatus)
        => Create(0, code, flightDate, creationDate, seatCount, observations, idFlight, idStatus);
}
