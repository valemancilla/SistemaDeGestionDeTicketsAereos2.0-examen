using SistemaDeGestionDeTicketsAereos.src.modules.role.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.role.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.role.Application.UseCases;

public sealed class GetAllRolesUseCase
{
    private readonly IRoleRepository _repo;
    public GetAllRolesUseCase(IRoleRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<Role>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.ListAsync(ct);
}
