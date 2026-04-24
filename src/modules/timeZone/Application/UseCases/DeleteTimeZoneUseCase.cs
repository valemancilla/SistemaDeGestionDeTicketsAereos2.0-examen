using SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Application.UseCases;

public sealed class DeleteTimeZoneUseCase
{
    private readonly ITimeZoneRepository _repo;
    public DeleteTimeZoneUseCase(ITimeZoneRepository repo) => _repo = repo;

    public async Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(TimeZoneId.Create(id), ct);
        if (existing is null) return false;
        await _repo.DeleteAsync(TimeZoneId.Create(id), ct);
        return true;
    }
}
