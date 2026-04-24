using SistemaDeGestionDeTicketsAereos.src.modules.user.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.user.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.user.Application.UseCases;

public sealed class GetAllUsersUseCase
{
    private readonly IUserRepository _repo;
    public GetAllUsersUseCase(IUserRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<User>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.ListAsync(ct);
}
