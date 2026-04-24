using SistemaDeGestionDeTicketsAereos.src.modules.fare.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.fare.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.fare.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.fare.Application.UseCases;

public sealed class UpdateFareUseCase
{
    private readonly IFareRepository _repo;
    public UpdateFareUseCase(IFareRepository repo) => _repo = repo;

    public async Task<Fare> ExecuteAsync(int id, string name, decimal basePrice, DateOnly validFrom, DateOnly validTo, DateOnly? expirationDate, int idAirline, bool active, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(FareId.Create(id), ct);
        if (existing is null) throw new KeyNotFoundException($"Fare with id '{id}' was not found.");
        var updated = Fare.Create(id, name, basePrice, validFrom, validTo, expirationDate, idAirline, active);
        await _repo.UpdateAsync(updated, ct);
        return updated;
    }
}
