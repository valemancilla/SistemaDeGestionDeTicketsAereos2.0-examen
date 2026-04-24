// Contrato del servicio de historial de estados de reserva: define las operaciones para la capa de presentación
using SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Application.Interfaces;

// Interfaz del servicio de historial — desacopla la capa UI del servicio concreto
public interface IBookingStatusHistoryService
{
    // Registra una nueva transición de estado en la reserva, con observación opcional
    Task<BookingStatusHistory> CreateAsync(DateTime changeDate, string? observation, int idBooking, int idStatus, int idUser, CancellationToken cancellationToken = default);

    // Busca un registro de historial por su ID, retorna null si no existe
    Task<BookingStatusHistory?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    // Retorna todas las transiciones de estado registradas en el sistema
    Task<IReadOnlyCollection<BookingStatusHistory>> GetAllAsync(CancellationToken cancellationToken = default);

    // Actualiza un registro de historial existente, lanza excepción si no se encuentra
    Task<BookingStatusHistory> UpdateAsync(int id, DateTime changeDate, string? observation, int idBooking, int idStatus, int idUser, CancellationToken cancellationToken = default);

    // Elimina un registro de historial por su ID, retorna false si no existe
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
