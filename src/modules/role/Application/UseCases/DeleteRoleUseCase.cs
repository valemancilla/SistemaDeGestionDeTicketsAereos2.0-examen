using SistemaDeGestionDeTicketsAereos.src.modules.role.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.role.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.role.Application.UseCases;

public sealed class DeleteRoleUseCase
{
    private readonly IRoleRepository _repo;
    public DeleteRoleUseCase(IRoleRepository repo) => _repo = repo;

    public async Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(RoleId.Create(id), ct);
        if (existing is null) return false;
        await _repo.DeleteAsync(RoleId.Create(id), ct);
        return true;
    }
}
