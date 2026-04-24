using SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Application.UseCases;

public sealed class CreateTimeZoneUseCase
{
    private readonly ITimeZoneRepository _repo;
    public CreateTimeZoneUseCase(ITimeZoneRepository repo) => _repo = repo;

    public async Task<AirlineTimeZone> ExecuteAsync(string name, string utcOffset, CancellationToken ct = default)
    {
        var entity = AirlineTimeZone.CreateNew(name, utcOffset);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}
