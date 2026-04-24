using SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Application.UseCases;

public sealed class GetSeatClassByIdUseCase
{
    private readonly ISeatClassRepository _repo;
    public GetSeatClassByIdUseCase(ISeatClassRepository repo) => _repo = repo;

    public async Task<SeatClass> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(SeatClassId.Create(id), ct);
        if (entity is null) throw new KeyNotFoundException($"SeatClass with id '{id}' was not found.");
        return entity;
    }
}
