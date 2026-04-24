using SistemaDeGestionDeTicketsAereos.src.modules.gender.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.gender.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.gender.Application.UseCases;

public sealed class DeleteGenderUseCase
{
    private readonly IGenderRepository _repo;
    public DeleteGenderUseCase(IGenderRepository repo) => _repo = repo;

    public async Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(GenderId.Create(id), ct);
        if (existing is null) return false;
        await _repo.DeleteAsync(GenderId.Create(id), ct);
        return true;
    }
}
