using SistemaDeGestionDeTicketsAereos.src.modules.fare.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.fare.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.fare.Application.UseCases;

public sealed class DeleteFareUseCase
{
    private readonly IFareRepository _repo;
    public DeleteFareUseCase(IFareRepository repo) => _repo = repo;

    public async Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(FareId.Create(id), ct);
        if (existing is null) return false;
        await _repo.DeleteAsync(FareId.Create(id), ct);
        return true;
    }
}
