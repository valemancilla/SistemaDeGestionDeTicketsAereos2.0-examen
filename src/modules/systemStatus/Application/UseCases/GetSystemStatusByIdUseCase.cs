using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Application.UseCases;

public sealed class GetSystemStatusByIdUseCase
{
    private readonly ISystemStatusRepository _repo;
    public GetSystemStatusByIdUseCase(ISystemStatusRepository repo) => _repo = repo;

    public async Task<SystemStatus> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(SystemStatusId.Create(id), ct);
        if (entity is null) throw new KeyNotFoundException($"SystemStatus with id '{id}' was not found.");
        return entity;
    }
}
