// Contrato del servicio de reservas: define las operaciones de negocio disponibles para la capa de presentación
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.booking.Application.Interfaces;

// Interfaz del servicio de reservas — desacopla la capa UI del servicio concreto
public interface IBookingService
{
    // Crea una nueva reserva validando que el código no exista previamente
    Task<Booking> CreateAsync(string code, DateTime flightDate, DateOnly creationDate, int seatCount, string? observations, int idFlight, int idStatus, CancellationToken cancellationToken = default);

    // Busca una reserva por su ID, retorna null si no existe
    Task<Booking?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    // Retorna todas las reservas del sistema
    Task<IReadOnlyCollection<Booking>> GetAllAsync(CancellationToken cancellationToken = default);

    // Actualiza una reserva existente, lanza excepción si no se encuentra
    Task<Booking> UpdateAsync(int id, string code, DateTime flightDate, DateOnly creationDate, int seatCount, string? observations, int idFlight, int idStatus, CancellationToken cancellationToken = default);

    // Elimina una reserva por su ID, retorna false si no existe
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
