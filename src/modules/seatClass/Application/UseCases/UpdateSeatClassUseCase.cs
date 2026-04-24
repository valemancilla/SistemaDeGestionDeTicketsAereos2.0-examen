using SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Application.UseCases;

public sealed class UpdateSeatClassUseCase
{
    private readonly ISeatClassRepository _repo;
    public UpdateSeatClassUseCase(ISeatClassRepository repo) => _repo = repo;

    public async Task<SeatClass> ExecuteAsync(int id, string name, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(SeatClassId.Create(id), ct);
        if (existing is null) throw new KeyNotFoundException($"SeatClass with id '{id}' was not found.");
        var updated = SeatClass.Create(id, name);
        await _repo.UpdateAsync(updated, ct);
        return updated;
    }
}
