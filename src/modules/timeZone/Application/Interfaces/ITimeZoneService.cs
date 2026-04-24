using SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Application.Interfaces;

public interface ITimeZoneService
{
    Task<AirlineTimeZone> CreateAsync(string name, string utcOffset, CancellationToken cancellationToken = default);

    Task<AirlineTimeZone?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<AirlineTimeZone>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<AirlineTimeZone> UpdateAsync(int id, string name, string utcOffset, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
