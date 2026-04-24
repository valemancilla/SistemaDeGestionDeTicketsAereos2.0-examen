using SistemaDeGestionDeTicketsAereos.src.modules.seat.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.seat.Application.UseCases;

public sealed class UpdateSeatUseCase
{
    private readonly ISeatRepository _repo;
    public UpdateSeatUseCase(ISeatRepository repo) => _repo = repo;

    public async Task<Seat> ExecuteAsync(int id, string number, int idAircraft, int idClase, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(SeatId.Create(id), ct);
        if (existing is null) throw new KeyNotFoundException($"Seat with id '{id}' was not found.");
        var updated = Seat.Create(id, number, idAircraft, idClase);
        await _repo.UpdateAsync(updated, ct);
        return updated;
    }
}
