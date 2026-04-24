using SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Application.UseCases;

public sealed class DeleteSeatClassUseCase
{
    private readonly ISeatClassRepository _repo;
    public DeleteSeatClassUseCase(ISeatClassRepository repo) => _repo = repo;

    public async Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(SeatClassId.Create(id), ct);
        if (existing is null) return false;
        await _repo.DeleteAsync(SeatClassId.Create(id), ct);
        return true;
    }
}
