// Contrato del repositorio de historial de estados de reservas: define las operaciones de persistencia disponibles
using SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Domain.Repositories;

// Interfaz que define las operaciones CRUD del repositorio de historial de estados de reservas
public interface IBookingStatusHistoryRepository
{
    // Busca un registro del historial por su ID
    Task<BookingStatusHistory?> GetByIdAsync(BookingStatusHistoryId id, CancellationToken ct = default);

    // Retorna todos los registros del historial del sistema
    Task<IReadOnlyList<BookingStatusHistory>> ListAsync(CancellationToken ct = default);

    // Retorna el historial completo de estados de una reserva específica
    Task<IReadOnlyList<BookingStatusHistory>> ListByBookingAsync(int idBooking, CancellationToken ct = default);

    // Agrega un nuevo registro al historial de estados
    Task AddAsync(BookingStatusHistory history, CancellationToken ct = default);

    // Actualiza un registro del historial existente
    Task UpdateAsync(BookingStatusHistory history, CancellationToken ct = default);

    // Elimina un registro del historial por su ID
    Task DeleteAsync(BookingStatusHistoryId id, CancellationToken ct = default);
}
