using SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Application.UseCases;

public sealed class GetAllSeatFlightsUseCase
{
    private readonly ISeatFlightRepository _repo;
    public GetAllSeatFlightsUseCase(ISeatFlightRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<SeatFlight>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.ListAsync(ct);
}
