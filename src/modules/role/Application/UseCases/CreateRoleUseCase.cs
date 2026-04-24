using SistemaDeGestionDeTicketsAereos.src.modules.role.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.role.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.role.Application.UseCases;

public sealed class CreateRoleUseCase
{
    private readonly IRoleRepository _repo;
    public CreateRoleUseCase(IRoleRepository repo) => _repo = repo;

    public async Task<Role> ExecuteAsync(string name, CancellationToken ct = default)
    {
        var entity = Role.CreateNew(name);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}
