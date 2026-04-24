// El pago registra la transacción económica que respalda una reserva — sin pago no hay tiquete
using SistemaDeGestionDeTicketsAereos.src.modules.payment.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.payment.Domain.aggregate;

// Agregado Payment: encapsula las reglas de negocio de un pago realizado en el sistema
public class Payment
{
    // ID del pago (Value Object)
    public PaymentId Id { get; private set; }

    // Monto total pagado — debe ser positivo
    public PaymentAmount Amount { get; private set; }

    // Fecha y hora exacta en que se realizó el pago
    public PaymentDate Date { get; private set; }

    // FK a la reserva que está siendo pagada
    public int IdBooking { get; private set; }

    // Opcional: tiquete concreto vinculado al pago
    public int? IdTicket { get; private set; }

    // FK al método de pago utilizado (efectivo, tarjeta, transferencia, etc.)
    public int IdPaymentMethod { get; private set; }

    // FK al estado actual del pago (pendiente, aprobado, rechazado, etc.)
    public int IdStatus { get; private set; }

    // Constructor privado: solo se crea a través del método Create
    private Payment(PaymentId id, PaymentAmount amount, PaymentDate date,
        int idBooking, int? idTicket, int idPaymentMethod, int idStatus)
    {
        Id = id;
        Amount = amount;
        Date = date;
        IdBooking = idBooking;
        IdTicket = idTicket;
        IdPaymentMethod = idPaymentMethod;
        IdStatus = idStatus;
    }

    // Método de fábrica para crear o reconstruir un pago desde la base de datos
    public static Payment Create(int id, decimal amount, DateTime date,
        int idBooking, int idPaymentMethod, int idStatus, int? idTicket = null)
    {
        // Regla: el pago debe estar asociado a una reserva válida
        if (idBooking <= 0)
            throw new ArgumentException("IdBooking must be greater than 0.", nameof(idBooking));

        if (idTicket is int tid && tid <= 0)
            throw new ArgumentException("IdTicket must be greater than 0 when provided.", nameof(idTicket));

        // Regla: el pago debe realizarse con un método de pago válido
        if (idPaymentMethod <= 0)
            throw new ArgumentException("IdPaymentMethod must be greater than 0.", nameof(idPaymentMethod));

        // Regla: el estado del pago debe ser una referencia válida
        if (idStatus <= 0)
            throw new ArgumentException("IdStatus must be greater than 0.", nameof(idStatus));

        // Regla: la fecha del pago no puede ser futura
        if (date > DateTime.Now)
            throw new ArgumentException("Payment date cannot be in the future.", nameof(date));

        // Regla: el monto es validado por su Value Object (debe ser mayor a cero)
        return new Payment(
            PaymentId.Create(id),
            PaymentAmount.Create(amount),
            PaymentDate.Create(date),
            idBooking,
            idTicket,
            idPaymentMethod,
            idStatus
        );
    }

    // Método de fábrica para crear un pago nuevo (ID = 0, la BD lo asigna después)
    public static Payment CreateNew(decimal amount, DateTime date, int idBooking, int idPaymentMethod, int idStatus, int? idTicket = null)
        => Create(0, amount, date, idBooking, idPaymentMethod, idStatus, idTicket);
}
