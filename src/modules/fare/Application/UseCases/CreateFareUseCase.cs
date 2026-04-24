using SistemaDeGestionDeTicketsAereos.src.modules.fare.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.fare.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.fare.Application.UseCases;

public sealed class CreateFareUseCase
{
    private readonly IFareRepository _repo;
    public CreateFareUseCase(IFareRepository repo) => _repo = repo;

    public async Task<Fare> ExecuteAsync(string name, decimal basePrice, DateOnly validFrom, DateOnly validTo, DateOnly? expirationDate, int idAirline, bool active, CancellationToken ct = default)
    {
        var entity = Fare.CreateNew(name, basePrice, validFrom, validTo, expirationDate, idAirline, active);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}
