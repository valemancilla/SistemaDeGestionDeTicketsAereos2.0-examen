// Contrato del repositorio de reservas: define todas las operaciones de persistencia disponibles para Booking
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.Repositories;

public interface IBookingRepository
{
    // Busca una reserva por su ID único
    Task<Booking?> GetByIdAsync(BookingId id, CancellationToken ct = default);

    // Busca una reserva por su código alfanumérico (el que se le entrega al cliente)
    Task<Booking?> GetByCodeAsync(string bookingCode, CancellationToken ct = default);

    // Devuelve todas las reservas del sistema
    Task<IReadOnlyList<Booking>> ListAsync(CancellationToken ct = default);

    // Persiste una nueva reserva en la base de datos
    Task AddAsync(Booking booking, CancellationToken ct = default);

    // Actualiza los datos de una reserva existente
    Task UpdateAsync(Booking booking, CancellationToken ct = default);

    // Elimina una reserva por su ID
    Task DeleteAsync(BookingId id, CancellationToken ct = default);
}
