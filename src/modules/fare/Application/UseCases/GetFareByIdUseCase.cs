using SistemaDeGestionDeTicketsAereos.src.modules.fare.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.fare.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.fare.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.fare.Application.UseCases;

public sealed class GetFareByIdUseCase
{
    private readonly IFareRepository _repo;
    public GetFareByIdUseCase(IFareRepository repo) => _repo = repo;

    public async Task<Fare> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(FareId.Create(id), ct);
        if (entity is null) throw new KeyNotFoundException($"Fare with id '{id}' was not found.");
        return entity;
    }
}
