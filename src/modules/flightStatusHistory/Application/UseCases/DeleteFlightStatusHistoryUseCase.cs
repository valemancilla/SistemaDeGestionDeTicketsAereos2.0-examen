using SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Application.UseCases;

public sealed class DeleteFlightStatusHistoryUseCase
{
    private readonly IFlightStatusHistoryRepository _repo;
    public DeleteFlightStatusHistoryUseCase(IFlightStatusHistoryRepository repo) => _repo = repo;

    public async Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(FlightStatusHistoryId.Create(id), ct);
        if (existing is null) return false;
        await _repo.DeleteAsync(FlightStatusHistoryId.Create(id), ct);
        return true;
    }
}
