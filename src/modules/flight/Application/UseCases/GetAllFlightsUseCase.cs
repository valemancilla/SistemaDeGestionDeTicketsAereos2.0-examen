using SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.flight.Application.UseCases;

public sealed class GetAllFlightsUseCase
{
    private readonly IFlightRepository _repo;
    public GetAllFlightsUseCase(IFlightRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<Flight>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.ListAsync(ct);
}
