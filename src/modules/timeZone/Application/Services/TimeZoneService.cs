using SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.timeZone.Application.Services;

public sealed class TimeZoneService : ITimeZoneService
{
    private readonly ITimeZoneRepository _timeZoneRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TimeZoneService(ITimeZoneRepository timeZoneRepository, IUnitOfWork unitOfWork)
    {
        _timeZoneRepository = timeZoneRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AirlineTimeZone> CreateAsync(string name, string utcOffset, CancellationToken cancellationToken = default)
    {
        var entity = AirlineTimeZone.CreateNew(name, utcOffset);
        await _timeZoneRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public Task<AirlineTimeZone?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _timeZoneRepository.GetByIdAsync(TimeZoneId.Create(id), cancellationToken);
    }

    public async Task<IReadOnlyCollection<AirlineTimeZone>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _timeZoneRepository.ListAsync(cancellationToken);
    }

    public async Task<AirlineTimeZone> UpdateAsync(int id, string name, string utcOffset, CancellationToken cancellationToken = default)
    {
        var timeZoneId = TimeZoneId.Create(id);
        var existing = await _timeZoneRepository.GetByIdAsync(timeZoneId, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"TimeZone with id '{id}' was not found.");

        var updated = AirlineTimeZone.Create(id, name, utcOffset);
        await _timeZoneRepository.UpdateAsync(updated, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return updated;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var timeZoneId = TimeZoneId.Create(id);
        var existing = await _timeZoneRepository.GetByIdAsync(timeZoneId, cancellationToken);
        if (existing is null)
            return false;

        await _timeZoneRepository.DeleteAsync(timeZoneId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
