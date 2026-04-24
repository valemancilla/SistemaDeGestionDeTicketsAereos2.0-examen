using SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.flightStatusHistory.Application.Services;

public sealed class FlightStatusHistoryService : IFlightStatusHistoryService
{
    private readonly IFlightStatusHistoryRepository _flightStatusHistoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public FlightStatusHistoryService(IFlightStatusHistoryRepository flightStatusHistoryRepository, IUnitOfWork unitOfWork)
    {
        _flightStatusHistoryRepository = flightStatusHistoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<FlightStatusHistory> CreateAsync(DateTime changeDate, string? observation, int idFlight, int idStatus, int idUser, CancellationToken cancellationToken = default)
    {
        var entity = FlightStatusHistory.CreateNew(changeDate, observation, idFlight, idStatus, idUser);
        await _flightStatusHistoryRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public Task<FlightStatusHistory?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _flightStatusHistoryRepository.GetByIdAsync(FlightStatusHistoryId.Create(id), cancellationToken);
    }

    public async Task<IReadOnlyCollection<FlightStatusHistory>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _flightStatusHistoryRepository.ListAsync(cancellationToken);
    }

    public async Task<FlightStatusHistory> UpdateAsync(int id, DateTime changeDate, string? observation, int idFlight, int idStatus, int idUser, CancellationToken cancellationToken = default)
    {
        var historyId = FlightStatusHistoryId.Create(id);
        var existing = await _flightStatusHistoryRepository.GetByIdAsync(historyId, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"FlightStatusHistory with id '{id}' was not found.");

        var updated = FlightStatusHistory.Create(id, changeDate, observation, idFlight, idStatus, idUser);
        await _flightStatusHistoryRepository.UpdateAsync(updated, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return updated;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var historyId = FlightStatusHistoryId.Create(id);
        var existing = await _flightStatusHistoryRepository.GetByIdAsync(historyId, cancellationToken);
        if (existing is null)
            return false;

        await _flightStatusHistoryRepository.DeleteAsync(historyId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
