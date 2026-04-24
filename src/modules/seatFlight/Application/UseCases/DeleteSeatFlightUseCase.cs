using SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Application.UseCases;

public sealed class DeleteSeatFlightUseCase
{
    private readonly ISeatFlightRepository _repo;
    public DeleteSeatFlightUseCase(ISeatFlightRepository repo) => _repo = repo;

    public async Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(SeatFlightId.Create(id), ct);
        if (existing is null) return false;
        await _repo.DeleteAsync(SeatFlightId.Create(id), ct);
        return true;
    }
}
