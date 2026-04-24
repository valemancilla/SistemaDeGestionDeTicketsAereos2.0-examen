using SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Application.UseCases;

public sealed class CreateSeatClassUseCase
{
    private readonly ISeatClassRepository _repo;
    public CreateSeatClassUseCase(ISeatClassRepository repo) => _repo = repo;

    public async Task<SeatClass> ExecuteAsync(string name, CancellationToken ct = default)
    {
        var entity = SeatClass.CreateNew(name);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}
