using SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Application.UseCases;

public sealed class GetSeatFlightByIdUseCase
{
    private readonly ISeatFlightRepository _repo;
    public GetSeatFlightByIdUseCase(ISeatFlightRepository repo) => _repo = repo;

    public async Task<SeatFlight> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(SeatFlightId.Create(id), ct);
        if (entity is null) throw new KeyNotFoundException($"SeatFlight with id '{id}' was not found.");
        return entity;
    }
}
