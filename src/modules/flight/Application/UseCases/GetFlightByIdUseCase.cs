using SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.flight.Application.UseCases;

public sealed class GetFlightByIdUseCase
{
    private readonly IFlightRepository _repo;
    public GetFlightByIdUseCase(IFlightRepository repo) => _repo = repo;

    public async Task<Flight> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(FlightId.Create(id), ct);
        if (entity is null) throw new KeyNotFoundException($"Flight with id '{id}' was not found.");
        return entity;
    }
}
