using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Application.UseCases;

public sealed class DeleteSystemStatusUseCase
{
    private readonly ISystemStatusRepository _repo;
    public DeleteSystemStatusUseCase(ISystemStatusRepository repo) => _repo = repo;

    public async Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(SystemStatusId.Create(id), ct);
        if (existing is null) return false;
        await _repo.DeleteAsync(SystemStatusId.Create(id), ct);
        return true;
    }
}
