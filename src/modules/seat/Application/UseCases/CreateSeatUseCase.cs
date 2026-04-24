using SistemaDeGestionDeTicketsAereos.src.modules.seat.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.seat.Application.UseCases;

public sealed class CreateSeatUseCase
{
    private readonly ISeatRepository _repo;
    public CreateSeatUseCase(ISeatRepository repo) => _repo = repo;

    public async Task<Seat> ExecuteAsync(string number, int idAircraft, int idClase, CancellationToken ct = default)
    {
        var entity = Seat.CreateNew(number, idAircraft, idClase);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}
