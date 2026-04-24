using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Application.Services;

public sealed class SystemStatusService : ISystemStatusService
{
    private readonly ISystemStatusRepository _systemStatusRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SystemStatusService(ISystemStatusRepository systemStatusRepository, IUnitOfWork unitOfWork)
    {
        _systemStatusRepository = systemStatusRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<SystemStatus> CreateAsync(string name, string entityType, CancellationToken cancellationToken = default)
    {
        var entity = SystemStatus.CreateNew(name, entityType);
        await _systemStatusRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public Task<SystemStatus?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _systemStatusRepository.GetByIdAsync(SystemStatusId.Create(id), cancellationToken);
    }

    public async Task<IReadOnlyCollection<SystemStatus>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _systemStatusRepository.ListAsync(cancellationToken);
    }

    public async Task<SystemStatus> UpdateAsync(int id, string name, string entityType, CancellationToken cancellationToken = default)
    {
        var statusId = SystemStatusId.Create(id);
        var existing = await _systemStatusRepository.GetByIdAsync(statusId, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"SystemStatus with id '{id}' was not found.");

        var updated = SystemStatus.Create(id, name, entityType);
        await _systemStatusRepository.UpdateAsync(updated, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return updated;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var statusId = SystemStatusId.Create(id);
        var existing = await _systemStatusRepository.GetByIdAsync(statusId, cancellationToken);
        if (existing is null)
            return false;

        await _systemStatusRepository.DeleteAsync(statusId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
