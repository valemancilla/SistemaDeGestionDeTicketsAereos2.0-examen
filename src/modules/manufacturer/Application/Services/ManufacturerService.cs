using SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Application.Services;

public sealed class ManufacturerService : IManufacturerService
{
    private readonly IManufacturerRepository _manufacturerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ManufacturerService(IManufacturerRepository manufacturerRepository, IUnitOfWork unitOfWork)
    {
        _manufacturerRepository = manufacturerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Manufacturer> CreateAsync(string name, CancellationToken cancellationToken = default)
    {
        var existing = await _manufacturerRepository.GetByNameAsync(name, cancellationToken);
        if (existing is not null)
            throw new InvalidOperationException($"Manufacturer '{name}' already exists.");

        var entity = Manufacturer.CreateNew(name);
        await _manufacturerRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public Task<Manufacturer?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _manufacturerRepository.GetByIdAsync(ManufacturerId.Create(id), cancellationToken);
    }

    public async Task<IReadOnlyCollection<Manufacturer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _manufacturerRepository.ListAsync(cancellationToken);
    }

    public async Task<Manufacturer> UpdateAsync(int id, string name, CancellationToken cancellationToken = default)
    {
        var manufacturerId = ManufacturerId.Create(id);
        var existing = await _manufacturerRepository.GetByIdAsync(manufacturerId, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"Manufacturer with id '{id}' was not found.");

        var updated = Manufacturer.Create(id, name);
        await _manufacturerRepository.UpdateAsync(updated, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return updated;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var manufacturerId = ManufacturerId.Create(id);
        var existing = await _manufacturerRepository.GetByIdAsync(manufacturerId, cancellationToken);
        if (existing is null)
            return false;

        await _manufacturerRepository.DeleteAsync(manufacturerId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
