using SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Application.UseCases;

public sealed class UpdateFlightStatusHistoryUseCase
{
    private readonly IFlightStatusHistoryRepository _repo;
    public UpdateFlightStatusHistoryUseCase(IFlightStatusHistoryRepository repo) => _repo = repo;

    public async Task<FlightStatusHistory> ExecuteAsync(int id, DateTime changeDate, string? observation, int idFlight, int idStatus, int idUser, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(FlightStatusHistoryId.Create(id), ct);
        if (existing is null) throw new KeyNotFoundException($"FlightStatusHistory with id '{id}' was not found.");
        var updated = FlightStatusHistory.Create(id, changeDate, observation, idFlight, idStatus, idUser);
        await _repo.UpdateAsync(updated, ct);
        return updated;
    }
}
