// La cancelación de reserva registra cuándo, por qué y con qué penalización se anuló una reserva
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Domain.aggregate;

// Agregado BookingCancellation: guarda el historial de cancelaciones con su motivo y monto de penalidad
public class BookingCancellation
{
    // ID del registro de cancelación
    public BookingCancellationId Id { get; private set; }

    // Fecha y hora en que se realizó la cancelación
    public BookingCancellationDate CancellationDate { get; private set; }

    // Motivo por el que se canceló la reserva (obligatorio)
    public BookingCancellationReason Reason { get; private set; }

    // Monto de penalización cobrado por la cancelación (puede ser 0 si no aplica)
    public BookingCancellationPenaltyAmount PenaltyAmount { get; private set; }

    // Referencia a la reserva que fue cancelada
    public int IdBooking { get; private set; }

    // Usuario que procesó la cancelación en el sistema
    public int IdUser { get; private set; }

    // Constructor privado para que solo se pueda crear a través del método Create
    private BookingCancellation(BookingCancellationId id, BookingCancellationDate cancellationDate,
        BookingCancellationReason reason, BookingCancellationPenaltyAmount penaltyAmount,
        int idBooking, int idUser)
    {
        Id = id;
        CancellationDate = cancellationDate;
        Reason = reason;
        PenaltyAmount = penaltyAmount;
        IdBooking = idBooking;
        IdUser = idUser;
    }

    // Método de fábrica que valida las reglas de negocio antes de crear la cancelación
    public static BookingCancellation Create(int id, DateTime cancellationDate,
        string reason, decimal penaltyAmount, int idBooking, int idUser)
    {
        // Regla: la cancelación debe estar asociada a una reserva válida
        if (idBooking <= 0)
            throw new ArgumentException("IdBooking must be greater than 0.", nameof(idBooking));

        // Regla: la cancelación debe ser registrada por un usuario válido del sistema
        if (idUser <= 0)
            throw new ArgumentException("IdUser must be greater than 0.", nameof(idUser));

        // Regla: la fecha de cancelación no puede ser futura
        if (cancellationDate > DateTime.Now)
            throw new ArgumentException("Cancellation date cannot be in the future.", nameof(cancellationDate));

        // Regla: motivo y monto de penalización son validados por sus Value Objects
        return new BookingCancellation(
            BookingCancellationId.Create(id),
            BookingCancellationDate.Create(cancellationDate),
            BookingCancellationReason.Create(reason),
            BookingCancellationPenaltyAmount.Create(penaltyAmount),
            idBooking,
            idUser
        );
    }

    // Crea una cancelación nueva con ID 0 (la BD lo asigna después)
    public static BookingCancellation CreateNew(DateTime cancellationDate, string reason, decimal penaltyAmount, int idBooking, int idUser)
        => Create(0, cancellationDate, reason, penaltyAmount, idBooking, idUser);
}
