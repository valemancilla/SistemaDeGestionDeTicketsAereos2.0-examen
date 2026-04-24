using SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Application.UseCases;

public sealed class CreateSeatFlightUseCase
{
    private readonly ISeatFlightRepository _repo;
    public CreateSeatFlightUseCase(ISeatFlightRepository repo) => _repo = repo;

    public async Task<SeatFlight> ExecuteAsync(int idSeat, int idFlight, bool available, CancellationToken ct = default)
    {
        var existing = await _repo.GetBySeatAndFlightAsync(idSeat, idFlight, ct);
        if (existing is not null) throw new InvalidOperationException($"SeatFlight for seat '{idSeat}' and flight '{idFlight}' already exists.");
        var entity = SeatFlight.CreateNew(idSeat, idFlight, available);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}
