using SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Application.Interfaces;

public interface IPersonAddressService
{
    Task<PersonAddress> CreateAsync(string street, string number, string neighborhood, string dwellingType, string? zipCode, int idPerson, int idCity, bool active, CancellationToken cancellationToken = default);

    Task<PersonAddress?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<PersonAddress>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<PersonAddress> UpdateAsync(int id, string street, string number, string neighborhood, string dwellingType, string? zipCode, int idPerson, int idCity, bool active, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
