using SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Application.UseCases;

public sealed class UpdateSeatFlightUseCase
{
    private readonly ISeatFlightRepository _repo;
    public UpdateSeatFlightUseCase(ISeatFlightRepository repo) => _repo = repo;

    public async Task<SeatFlight> ExecuteAsync(int id, int idSeat, int idFlight, bool available, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(SeatFlightId.Create(id), ct);
        if (existing is null) throw new KeyNotFoundException($"SeatFlight with id '{id}' was not found.");
        var updated = SeatFlight.Create(id, idSeat, idFlight, available);
        await _repo.UpdateAsync(updated, ct);
        return updated;
    }
}
