// Contrato del servicio de canales de check-in: define las operaciones para la capa de presentación
using SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Application.Interfaces;

// Interfaz del servicio de canales — desacopla la capa UI del servicio concreto
public interface ICheckInChannelService
{
    // Registra un nuevo canal de check-in (web, mostrador, app móvil, etc.)
    Task<CheckInChannel> CreateAsync(string name, CancellationToken cancellationToken = default);

    // Busca un canal por su ID, retorna null si no existe
    Task<CheckInChannel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    // Retorna todos los canales de check-in registrados
    Task<IReadOnlyCollection<CheckInChannel>> GetAllAsync(CancellationToken cancellationToken = default);

    // Actualiza el nombre de un canal existente, lanza excepción si no se encuentra
    Task<CheckInChannel> UpdateAsync(int id, string name, CancellationToken cancellationToken = default);

    // Elimina un canal por su ID, retorna false si no existe
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
