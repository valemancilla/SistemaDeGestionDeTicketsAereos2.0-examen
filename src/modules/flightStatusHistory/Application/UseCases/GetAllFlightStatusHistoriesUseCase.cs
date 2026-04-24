using SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Application.UseCases;

public sealed class GetAllFlightStatusHistoriesUseCase
{
    private readonly IFlightStatusHistoryRepository _repo;
    public GetAllFlightStatusHistoriesUseCase(IFlightStatusHistoryRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<FlightStatusHistory>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.ListAsync(ct);
}
