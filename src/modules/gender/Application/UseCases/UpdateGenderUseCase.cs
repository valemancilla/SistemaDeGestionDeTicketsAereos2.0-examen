using SistemaDeGestionDeTicketsAereos.src.modules.gender.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.gender.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.gender.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.gender.Application.UseCases;

public sealed class UpdateGenderUseCase
{
    private readonly IGenderRepository _repo;
    public UpdateGenderUseCase(IGenderRepository repo) => _repo = repo;

    public async Task<Gender> ExecuteAsync(int id, string description, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(GenderId.Create(id), ct);
        if (existing is null) throw new KeyNotFoundException($"Gender with id '{id}' was not found.");
        var updated = Gender.Create(id, description);
        await _repo.UpdateAsync(updated, ct);
        return updated;
    }
}
