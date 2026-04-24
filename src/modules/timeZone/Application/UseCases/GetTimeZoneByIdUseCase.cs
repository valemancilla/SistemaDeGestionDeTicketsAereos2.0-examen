using SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Application.UseCases;

public sealed class GetTimeZoneByIdUseCase
{
    private readonly ITimeZoneRepository _repo;
    public GetTimeZoneByIdUseCase(ITimeZoneRepository repo) => _repo = repo;

    public async Task<AirlineTimeZone> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(TimeZoneId.Create(id), ct);
        if (entity is null) throw new KeyNotFoundException($"TimeZone with id '{id}' was not found.");
        return entity;
    }
}
