using SistemaDeGestionDeTicketsAereos.src.modules.user.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.user.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.user.Application.UseCases;

public sealed class DeleteUserUseCase
{
    private readonly IUserRepository _repo;
    public DeleteUserUseCase(IUserRepository repo) => _repo = repo;

    public async Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(UserId.Create(id), ct);
        if (existing is null) return false;
        await _repo.DeleteAsync(UserId.Create(id), ct);
        return true;
    }
}
