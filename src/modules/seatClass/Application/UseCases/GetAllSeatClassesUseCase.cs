using SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Application.UseCases;

public sealed class GetAllSeatClassesUseCase
{
    private readonly ISeatClassRepository _repo;
    public GetAllSeatClassesUseCase(ISeatClassRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<SeatClass>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.ListAsync(ct);
}
