using SistemaDeGestionDeTicketsAereos.src.modules.seat.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.seat.Application.UseCases;

public sealed class GetSeatByIdUseCase
{
    private readonly ISeatRepository _repo;
    public GetSeatByIdUseCase(ISeatRepository repo) => _repo = repo;

    public async Task<Seat> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(SeatId.Create(id), ct);
        if (entity is null) throw new KeyNotFoundException($"Seat with id '{id}' was not found.");
        return entity;
    }
}
