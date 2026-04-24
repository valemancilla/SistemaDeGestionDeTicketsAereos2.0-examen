using SistemaDeGestionDeTicketsAereos.src.modules.person.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.person.Application.Interfaces;

public interface IPersonService
{
    Task<Person> CreateAsync(string firstName, string lastName, DateOnly birthDate, string documentNumber, int idDocumentType, int idGender, int idCountry, int? idAddress, CancellationToken cancellationToken = default);

    Task<Person?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Person>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Person> UpdateAsync(int id, string firstName, string lastName, DateOnly birthDate, string documentNumber, int idDocumentType, int idGender, int idCountry, int? idAddress, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
