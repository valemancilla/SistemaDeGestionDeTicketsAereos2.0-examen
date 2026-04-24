// Contrato del repositorio de cancelaciones de reservas: define las operaciones de persistencia disponibles
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Domain.Repositories;

public interface IBookingCancellationRepository
{
    // Busca una cancelación por su ID
    Task<BookingCancellation?> GetByIdAsync(BookingCancellationId id, CancellationToken ct = default);

    // Devuelve todas las cancelaciones registradas en el sistema
    Task<IReadOnlyList<BookingCancellation>> ListAsync(CancellationToken ct = default);

    // Devuelve todas las cancelaciones asociadas a una reserva específica
    Task<IReadOnlyList<BookingCancellation>> ListByBookingAsync(int idBooking, CancellationToken ct = default);

    // Persiste una nueva cancelación en la base de datos
    Task AddAsync(BookingCancellation cancellation, CancellationToken ct = default);

    // Actualiza los datos de una cancelación existente
    Task UpdateAsync(BookingCancellation cancellation, CancellationToken ct = default);

    // Elimina una cancelación por su ID
    Task DeleteAsync(BookingCancellationId id, CancellationToken ct = default);
}
