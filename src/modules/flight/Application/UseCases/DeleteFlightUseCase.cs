using SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.flight.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.flight.Application.UseCases;

public sealed class DeleteFlightUseCase
{
    private readonly IFlightRepository _repo;
    public DeleteFlightUseCase(IFlightRepository repo) => _repo = repo;

    public async Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(FlightId.Create(id), ct);
        if (existing is null) return false;
        await _repo.DeleteAsync(FlightId.Create(id), ct);
        return true;
    }
}
