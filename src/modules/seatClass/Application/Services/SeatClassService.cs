using SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Application.Services;

public sealed class SeatClassService : ISeatClassService
{
    private readonly ISeatClassRepository _seatClassRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SeatClassService(ISeatClassRepository seatClassRepository, IUnitOfWork unitOfWork)
    {
        _seatClassRepository = seatClassRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<SeatClass> CreateAsync(string name, CancellationToken cancellationToken = default)
    {
        var entity = SeatClass.CreateNew(name);
        await _seatClassRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public Task<SeatClass?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _seatClassRepository.GetByIdAsync(SeatClassId.Create(id), cancellationToken);
    }

    public async Task<IReadOnlyCollection<SeatClass>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _seatClassRepository.ListAsync(cancellationToken);
    }

    public async Task<SeatClass> UpdateAsync(int id, string name, CancellationToken cancellationToken = default)
    {
        var seatClassId = SeatClassId.Create(id);
        var existing = await _seatClassRepository.GetByIdAsync(seatClassId, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"SeatClass with id '{id}' was not found.");

        var updated = SeatClass.Create(id, name);
        await _seatClassRepository.UpdateAsync(updated, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return updated;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var seatClassId = SeatClassId.Create(id);
        var existing = await _seatClassRepository.GetByIdAsync(seatClassId, cancellationToken);
        if (existing is null)
            return false;

        await _seatClassRepository.DeleteAsync(seatClassId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
