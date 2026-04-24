using SistemaDeGestionDeTicketsAereos.src.modules.role.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.role.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.role.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.role.Application.UseCases;

public sealed class GetRoleByIdUseCase
{
    private readonly IRoleRepository _repo;
    public GetRoleByIdUseCase(IRoleRepository repo) => _repo = repo;

    public async Task<Role> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(RoleId.Create(id), ct);
        if (entity is null) throw new KeyNotFoundException($"Role with id '{id}' was not found.");
        return entity;
    }
}
