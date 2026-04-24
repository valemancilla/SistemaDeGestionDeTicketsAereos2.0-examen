// Contrato del servicio de cancelaciones de reserva: define las operaciones disponibles para la capa de presentación
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Application.Interfaces;

// Interfaz del servicio de cancelaciones — desacopla la capa UI del servicio concreto
public interface IBookingCancellationService
{
    // Registra una nueva cancelación con su motivo y posible penalización económica
    Task<BookingCancellation> CreateAsync(DateTime cancellationDate, string reason, decimal penaltyAmount, int idBooking, int idUser, CancellationToken cancellationToken = default);

    // Busca una cancelación por su ID, retorna null si no existe
    Task<BookingCancellation?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    // Retorna todas las cancelaciones registradas en el sistema
    Task<IReadOnlyCollection<BookingCancellation>> GetAllAsync(CancellationToken cancellationToken = default);

    // Actualiza los datos de una cancelación existente, lanza excepción si no se encuentra
    Task<BookingCancellation> UpdateAsync(int id, DateTime cancellationDate, string reason, decimal penaltyAmount, int idBooking, int idUser, CancellationToken cancellationToken = default);

    // Elimina una cancelación por su ID, retorna false si no existe
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
