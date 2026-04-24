using SistemaDeGestionDeTicketsAereos.src.modules.user.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.user.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.user.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.user.Application.UseCases;

public sealed class UpdateUserUseCase
{
    private readonly IUserRepository _repo;
    public UpdateUserUseCase(IUserRepository repo) => _repo = repo;

    public async Task<User> ExecuteAsync(int id, string username, string password, int idUserRole, int? idPerson, bool active, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(UserId.Create(id), ct);
        if (existing is null) throw new KeyNotFoundException($"User with id '{id}' was not found.");
        var updated = User.Create(id, username, password, idUserRole, idPerson, active);
        await _repo.UpdateAsync(updated, ct);
        return updated;
    }
}
