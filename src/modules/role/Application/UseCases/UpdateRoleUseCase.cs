using SistemaDeGestionDeTicketsAereos.src.modules.role.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.role.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.role.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.role.Application.UseCases;

public sealed class UpdateRoleUseCase
{
    private readonly IRoleRepository _repo;
    public UpdateRoleUseCase(IRoleRepository repo) => _repo = repo;

    public async Task<Role> ExecuteAsync(int id, string name, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(RoleId.Create(id), ct);
        if (existing is null) throw new KeyNotFoundException($"Role with id '{id}' was not found.");
        var updated = Role.Create(id, name);
        await _repo.UpdateAsync(updated, ct);
        return updated;
    }
}
