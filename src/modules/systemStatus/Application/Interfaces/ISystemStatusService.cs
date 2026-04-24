using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Application.Interfaces;

public interface ISystemStatusService
{
    Task<SystemStatus> CreateAsync(string name, string entityType, CancellationToken cancellationToken = default);

    Task<SystemStatus?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SystemStatus>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<SystemStatus> UpdateAsync(int id, string name, string entityType, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
