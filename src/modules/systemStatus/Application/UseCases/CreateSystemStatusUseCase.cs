using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Application.UseCases;

public sealed class CreateSystemStatusUseCase
{
    private readonly ISystemStatusRepository _repo;
    public CreateSystemStatusUseCase(ISystemStatusRepository repo) => _repo = repo;

    public async Task<SystemStatus> ExecuteAsync(string name, string entityType, CancellationToken ct = default)
    {
        var entity = SystemStatus.CreateNew(name, entityType);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}
