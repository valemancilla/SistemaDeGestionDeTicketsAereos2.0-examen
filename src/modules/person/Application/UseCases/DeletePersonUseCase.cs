using SistemaDeGestionDeTicketsAereos.src.modules.person.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.person.Application.UseCases;

public sealed class DeletePersonUseCase
{
    private readonly IPersonRepository _repo;
    public DeletePersonUseCase(IPersonRepository repo) => _repo = repo;

    public async Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(PersonId.Create(id), ct);
        if (existing is null) return false;
        await _repo.DeleteAsync(PersonId.Create(id), ct);
        return true;
    }
}
