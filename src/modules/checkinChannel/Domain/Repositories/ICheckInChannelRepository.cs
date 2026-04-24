// Contrato del repositorio de canales de check-in: define las operaciones de persistencia disponibles
using SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Domain.Repositories;

// Interfaz que define las operaciones CRUD del repositorio de canales de check-in
public interface ICheckInChannelRepository
{
    // Busca un canal de check-in por su ID
    Task<CheckInChannel?> GetByIdAsync(CheckInChannelId id, CancellationToken ct = default);

    // Retorna todos los canales de check-in disponibles
    Task<IReadOnlyList<CheckInChannel>> ListAsync(CancellationToken ct = default);

    // Agrega un nuevo canal de check-in al sistema
    Task AddAsync(CheckInChannel channel, CancellationToken ct = default);

    // Actualiza los datos de un canal existente
    Task UpdateAsync(CheckInChannel channel, CancellationToken ct = default);

    // Elimina un canal de check-in por su ID
    Task DeleteAsync(CheckInChannelId id, CancellationToken ct = default);
}
