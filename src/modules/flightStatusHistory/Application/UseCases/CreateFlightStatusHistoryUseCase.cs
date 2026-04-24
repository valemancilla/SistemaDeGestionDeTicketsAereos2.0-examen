using SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Application.UseCases;

public sealed class CreateFlightStatusHistoryUseCase
{
    private readonly IFlightStatusHistoryRepository _repo;
    public CreateFlightStatusHistoryUseCase(IFlightStatusHistoryRepository repo) => _repo = repo;

    public async Task<FlightStatusHistory> ExecuteAsync(DateTime changeDate, string? observation, int idFlight, int idStatus, int idUser, CancellationToken ct = default)
    {
        var entity = FlightStatusHistory.CreateNew(changeDate, observation, idFlight, idStatus, idUser);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}
