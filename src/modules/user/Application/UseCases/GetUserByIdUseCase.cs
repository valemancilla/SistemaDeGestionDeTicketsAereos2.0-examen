using SistemaDeGestionDeTicketsAereos.src.modules.user.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.user.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.user.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.user.Application.UseCases;

public sealed class GetUserByIdUseCase
{
    private readonly IUserRepository _repo;
    public GetUserByIdUseCase(IUserRepository repo) => _repo = repo;

    public async Task<User> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(UserId.Create(id), ct);
        if (entity is null) throw new KeyNotFoundException($"User with id '{id}' was not found.");
        return entity;
    }
}
