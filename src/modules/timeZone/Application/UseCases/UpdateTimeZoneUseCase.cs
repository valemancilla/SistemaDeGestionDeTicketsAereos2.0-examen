using SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Application.UseCases;

public sealed class UpdateTimeZoneUseCase
{
    private readonly ITimeZoneRepository _repo;
    public UpdateTimeZoneUseCase(ITimeZoneRepository repo) => _repo = repo;

    public async Task<AirlineTimeZone> ExecuteAsync(int id, string name, string utcOffset, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(TimeZoneId.Create(id), ct);
        if (existing is null) throw new KeyNotFoundException($"TimeZone with id '{id}' was not found.");
        var updated = AirlineTimeZone.Create(id, name, utcOffset);
        await _repo.UpdateAsync(updated, ct);
        return updated;
    }
}
