using SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Application.UseCases;

public sealed class GetAllTimeZonesUseCase
{
    private readonly ITimeZoneRepository _repo;
    public GetAllTimeZonesUseCase(ITimeZoneRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<AirlineTimeZone>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.ListAsync(ct);
}
