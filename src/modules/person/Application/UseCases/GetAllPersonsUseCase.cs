using SistemaDeGestionDeTicketsAereos.src.modules.person.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.person.Application.UseCases;

public sealed class GetAllPersonsUseCase
{
    private readonly IPersonRepository _repo;
    public GetAllPersonsUseCase(IPersonRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<Person>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.ListAsync(ct);
}
