using SistemaDeGestionDeTicketsAereos.src.modules.fare.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.fare.Application.Interfaces;

public interface IFareService
{
    Task<Fare> CreateAsync(string name, decimal basePrice, DateOnly validFrom, DateOnly validTo, DateOnly? expirationDate, int idAirline, bool active, CancellationToken cancellationToken = default);

    Task<Fare?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Fare>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Fare> UpdateAsync(int id, string name, decimal basePrice, DateOnly validFrom, DateOnly validTo, DateOnly? expirationDate, int idAirline, bool active, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
