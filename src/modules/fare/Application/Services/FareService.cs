using SistemaDeGestionDeTicketsAereos.src.modules.fare.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.fare.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.fare.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.fare.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.fare.Application.Services;

public sealed class FareService : IFareService
{
    private readonly IFareRepository _fareRepository;
    private readonly IUnitOfWork _unitOfWork;

    public FareService(IFareRepository fareRepository, IUnitOfWork unitOfWork)
    {
        _fareRepository = fareRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Fare> CreateAsync(string name, decimal basePrice, DateOnly validFrom, DateOnly validTo, DateOnly? expirationDate, int idAirline, bool active, CancellationToken cancellationToken = default)
    {
        var entity = Fare.CreateNew(name, basePrice, validFrom, validTo, expirationDate, idAirline, active);
        await _fareRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public Task<Fare?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _fareRepository.GetByIdAsync(FareId.Create(id), cancellationToken);
    }

    public async Task<IReadOnlyCollection<Fare>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _fareRepository.ListAsync(cancellationToken);
    }

    public async Task<Fare> UpdateAsync(int id, string name, decimal basePrice, DateOnly validFrom, DateOnly validTo, DateOnly? expirationDate, int idAirline, bool active, CancellationToken cancellationToken = default)
    {
        var fareId = FareId.Create(id);
        var existing = await _fareRepository.GetByIdAsync(fareId, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"Fare with id '{id}' was not found.");

        var updated = Fare.Create(id, name, basePrice, validFrom, validTo, expirationDate, idAirline, active);
        await _fareRepository.UpdateAsync(updated, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return updated;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var fareId = FareId.Create(id);
        var existing = await _fareRepository.GetByIdAsync(fareId, cancellationToken);
        if (existing is null)
            return false;

        await _fareRepository.DeleteAsync(fareId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
