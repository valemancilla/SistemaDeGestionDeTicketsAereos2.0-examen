using SistemaDeGestionDeTicketsAereos.src.modules.seat.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.seat.Application.UseCases;

public sealed class GetAllSeatsUseCase
{
    private readonly ISeatRepository _repo;
    public GetAllSeatsUseCase(ISeatRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<Seat>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.ListAsync(ct);
}
