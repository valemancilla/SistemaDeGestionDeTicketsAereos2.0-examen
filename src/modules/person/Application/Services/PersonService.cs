using SistemaDeGestionDeTicketsAereos.src.modules.person.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.person.Application.Services;

public sealed class PersonService : IPersonService
{
    private readonly IPersonRepository _personRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PersonService(IPersonRepository personRepository, IUnitOfWork unitOfWork)
    {
        _personRepository = personRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Person> CreateAsync(string firstName, string lastName, DateOnly birthDate, string documentNumber, int idDocumentType, int idGender, int idCountry, int? idAddress, CancellationToken cancellationToken = default)
    {
        var normalized = PersonDocumentNumber.Create(documentNumber).Value;
        var existing = await _personRepository.GetByDocumentNumberAsync(normalized, cancellationToken);
        if (existing is not null)
            throw new InvalidOperationException($"Ya existe una persona con el número de documento '{normalized}'.");

        var entity = Person.CreateNew(firstName, lastName, birthDate, documentNumber, idDocumentType, idGender, idCountry, idAddress);
        await _personRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public Task<Person?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _personRepository.GetByIdAsync(PersonId.Create(id), cancellationToken);
    }

    public async Task<IReadOnlyCollection<Person>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _personRepository.ListAsync(cancellationToken);
    }

    public async Task<Person> UpdateAsync(int id, string firstName, string lastName, DateOnly birthDate, string documentNumber, int idDocumentType, int idGender, int idCountry, int? idAddress, CancellationToken cancellationToken = default)
    {
        var personId = PersonId.Create(id);
        var existing = await _personRepository.GetByIdAsync(personId, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"Person with id '{id}' was not found.");

        var normalized = PersonDocumentNumber.Create(documentNumber).Value;
        var conflict = await _personRepository.GetByDocumentNumberAsync(normalized, cancellationToken);
        if (conflict is not null && conflict.Id.Value != id)
            throw new InvalidOperationException($"Ya existe otra persona con el número de documento '{normalized}'.");

        var updated = Person.Create(id, firstName, lastName, birthDate, documentNumber, idDocumentType, idGender, idCountry, idAddress);
        await _personRepository.UpdateAsync(updated, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return updated;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var personId = PersonId.Create(id);
        var existing = await _personRepository.GetByIdAsync(personId, cancellationToken);
        if (existing is null)
            return false;

        await _personRepository.DeleteAsync(personId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
