using SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Application.Services;

public sealed class PersonAddressService : IPersonAddressService
{
    private readonly IPersonAddressRepository _personAddressRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PersonAddressService(IPersonAddressRepository personAddressRepository, IUnitOfWork unitOfWork)
    {
        _personAddressRepository = personAddressRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PersonAddress> CreateAsync(string street, string number, string neighborhood, string dwellingType, string? zipCode, int idPerson, int idCity, bool active, CancellationToken cancellationToken = default)
    {
        var entity = PersonAddress.CreateNew(street, number, neighborhood, dwellingType, zipCode, idPerson, idCity, active);
        await _personAddressRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public Task<PersonAddress?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _personAddressRepository.GetByIdAsync(PersonAddressId.Create(id), cancellationToken);
    }

    public async Task<IReadOnlyCollection<PersonAddress>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _personAddressRepository.ListAsync(cancellationToken);
    }

    public async Task<PersonAddress> UpdateAsync(int id, string street, string number, string neighborhood, string dwellingType, string? zipCode, int idPerson, int idCity, bool active, CancellationToken cancellationToken = default)
    {
        var personAddressId = PersonAddressId.Create(id);
        var existing = await _personAddressRepository.GetByIdAsync(personAddressId, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"PersonAddress with id '{id}' was not found.");

        var updated = PersonAddress.Create(id, street, number, neighborhood, dwellingType, zipCode, idPerson, idCity, active);
        await _personAddressRepository.UpdateAsync(updated, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return updated;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var personAddressId = PersonAddressId.Create(id);
        var existing = await _personAddressRepository.GetByIdAsync(personAddressId, cancellationToken);
        if (existing is null)
            return false;

        await _personAddressRepository.DeleteAsync(personAddressId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
