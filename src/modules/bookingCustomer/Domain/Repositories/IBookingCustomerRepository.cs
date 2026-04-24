// Contrato del repositorio de pasajeros por reserva: define las operaciones de persistencia disponibles
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Domain.Repositories;

// Interfaz que define las operaciones CRUD del repositorio de BookingCustomer
public interface IBookingCustomerRepository
{
    // Busca un registro de pasajero-reserva por su ID
    Task<BookingCustomer?> GetByIdAsync(BookingCustomerId id, CancellationToken ct = default);

    // Retorna todos los registros de pasajeros del sistema
    Task<IReadOnlyList<BookingCustomer>> ListAsync(CancellationToken ct = default);

    // Retorna todos los pasajeros asociados a una reserva específica
    Task<IReadOnlyList<BookingCustomer>> ListByBookingAsync(int idBooking, CancellationToken ct = default);

    // Agrega un nuevo pasajero a una reserva
    Task AddAsync(BookingCustomer bookingCustomer, CancellationToken ct = default);

    // Actualiza los datos de un pasajero en una reserva existente (ej: cambio de asiento)
    Task UpdateAsync(BookingCustomer bookingCustomer, CancellationToken ct = default);

    // Elimina la asociación de un pasajero con una reserva
    Task DeleteAsync(BookingCustomerId id, CancellationToken ct = default);
}
