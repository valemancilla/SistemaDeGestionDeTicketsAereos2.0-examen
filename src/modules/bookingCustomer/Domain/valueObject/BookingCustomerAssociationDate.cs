namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Domain.valueObject;

// Value Object para la fecha en que se asoció un pasajero a una reserva
public sealed record BookingCustomerAssociationDate
{
    // Fecha y hora del momento en que se realizó la asociación
    public DateTime Value { get; }

    // Constructor privado: solo se crea a través del método Create
    private BookingCustomerAssociationDate(DateTime value) => Value = value;

    // Valida que la fecha sea real y no esté en el futuro
    public static BookingCustomerAssociationDate Create(DateTime value)
    {
        if (value == DateTime.MinValue)
            throw new ArgumentException("Association date cannot be empty.", nameof(value));

        // Un pasajero no puede ser asociado a una reserva con una fecha futura
        if (value > DateTime.Now)
            throw new ArgumentException("Association date cannot be in the future.", nameof(value));

        return new BookingCustomerAssociationDate(value);
    }

    public override string ToString() => Value.ToString("yyyy-MM-dd HH:mm");
}
