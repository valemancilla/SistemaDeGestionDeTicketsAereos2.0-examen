using SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Application.Interfaces;

public interface IManufacturerService
{
    Task<Manufacturer> CreateAsync(string name, CancellationToken cancellationToken = default);

    Task<Manufacturer?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Manufacturer>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Manufacturer> UpdateAsync(int id, string name, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
