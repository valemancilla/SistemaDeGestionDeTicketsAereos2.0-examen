using SistemaDeGestionDeTicketsAereos.src.modules.person.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.person.Application.UseCases;

public sealed class GetPersonByIdUseCase
{
    private readonly IPersonRepository _repo;
    public GetPersonByIdUseCase(IPersonRepository repo) => _repo = repo;

    public async Task<Person> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(PersonId.Create(id), ct);
        if (entity is null) throw new KeyNotFoundException($"Person with id '{id}' was not found.");
        return entity;
    }
}
