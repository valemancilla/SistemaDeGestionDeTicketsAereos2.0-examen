using SistemaDeGestionDeTicketsAereos.src.modules.gender.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.gender.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.gender.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.gender.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.gender.Application.Services;

public sealed class GenderService : IGenderService
{
    private readonly IGenderRepository _genderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GenderService(IGenderRepository genderRepository, IUnitOfWork unitOfWork)
    {
        _genderRepository = genderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Gender> CreateAsync(string description, CancellationToken cancellationToken = default)
    {
        var entity = Gender.CreateNew(description);
        await _genderRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public Task<Gender?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _genderRepository.GetByIdAsync(GenderId.Create(id), cancellationToken);
    }

    public async Task<IReadOnlyCollection<Gender>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _genderRepository.ListAsync(cancellationToken);
    }

    public async Task<Gender> UpdateAsync(int id, string description, CancellationToken cancellationToken = default)
    {
        var genderId = GenderId.Create(id);
        var existing = await _genderRepository.GetByIdAsync(genderId, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"Gender with id '{id}' was not found.");

        var updated = Gender.Create(id, description);
        await _genderRepository.UpdateAsync(updated, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return updated;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var genderId = GenderId.Create(id);
        var existing = await _genderRepository.GetByIdAsync(genderId, cancellationToken);
        if (existing is null)
            return false;

        await _genderRepository.DeleteAsync(genderId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
