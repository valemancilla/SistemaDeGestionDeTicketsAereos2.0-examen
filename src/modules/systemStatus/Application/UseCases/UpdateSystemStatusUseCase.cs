using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Application.UseCases;

public sealed class UpdateSystemStatusUseCase
{
    private readonly ISystemStatusRepository _repo;
    public UpdateSystemStatusUseCase(ISystemStatusRepository repo) => _repo = repo;

    public async Task<SystemStatus> ExecuteAsync(int id, string name, string entityType, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(SystemStatusId.Create(id), ct);
        if (existing is null) throw new KeyNotFoundException($"SystemStatus with id '{id}' was not found.");
        var updated = SystemStatus.Create(id, name, entityType);
        await _repo.UpdateAsync(updated, ct);
        return updated;
    }
}
