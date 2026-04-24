using SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Application.UseCases;

public sealed class GetFlightStatusHistoryByIdUseCase
{
    private readonly IFlightStatusHistoryRepository _repo;
    public GetFlightStatusHistoryByIdUseCase(IFlightStatusHistoryRepository repo) => _repo = repo;

    public async Task<FlightStatusHistory> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(FlightStatusHistoryId.Create(id), ct);
        if (entity is null) throw new KeyNotFoundException($"FlightStatusHistory with id '{id}' was not found.");
        return entity;
    }
}
