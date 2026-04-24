using SistemaDeGestionDeTicketsAereos.src.modules.user.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.user.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.user.Application.UseCases;

public sealed class CreateUserUseCase
{
    private readonly IUserRepository _repo;
    public CreateUserUseCase(IUserRepository repo) => _repo = repo;

    public async Task<User> ExecuteAsync(string username, string password, int idUserRole, int? idPerson, bool active, CancellationToken ct = default)
    {
        var existing = await _repo.GetUserByUsernameAsync(username, ct);
        if (existing is not null) throw new InvalidOperationException($"User with username '{username}' already exists.");
        var entity = User.CreateNew(username, password, idUserRole, idPerson, active);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}
