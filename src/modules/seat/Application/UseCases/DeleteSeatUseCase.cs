using SistemaDeGestionDeTicketsAereos.src.modules.seat.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.seat.Application.UseCases;

public sealed class DeleteSeatUseCase
{
    private readonly ISeatRepository _repo;
    public DeleteSeatUseCase(ISeatRepository repo) => _repo = repo;

    public async Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(SeatId.Create(id), ct);
        if (existing is null) return false;
        await _repo.DeleteAsync(SeatId.Create(id), ct);
        return true;
    }
}
