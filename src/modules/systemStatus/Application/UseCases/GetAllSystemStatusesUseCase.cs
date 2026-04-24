using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Application.UseCases;

public sealed class GetAllSystemStatusesUseCase
{
    private readonly ISystemStatusRepository _repo;
    public GetAllSystemStatusesUseCase(ISystemStatusRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<SystemStatus>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.ListAsync(ct);
}
