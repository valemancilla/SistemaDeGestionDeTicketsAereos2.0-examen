using SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Application.Services;

public sealed class TicketStatusHistoryService : ITicketStatusHistoryService
{
    private readonly ITicketStatusHistoryRepository _ticketStatusHistoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TicketStatusHistoryService(ITicketStatusHistoryRepository ticketStatusHistoryRepository, IUnitOfWork unitOfWork)
    {
        _ticketStatusHistoryRepository = ticketStatusHistoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TicketStatusHistory> CreateAsync(DateTime changeDate, string? observation, int idTicket, int idStatus, int idUser, CancellationToken cancellationToken = default)
    {
        var entity = TicketStatusHistory.CreateNew(changeDate, observation, idTicket, idStatus, idUser);
        await _ticketStatusHistoryRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public Task<TicketStatusHistory?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _ticketStatusHistoryRepository.GetByIdAsync(TicketStatusHistoryId.Create(id), cancellationToken);
    }

    public async Task<IReadOnlyCollection<TicketStatusHistory>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _ticketStatusHistoryRepository.ListAsync(cancellationToken);
    }

    public async Task<TicketStatusHistory> UpdateAsync(int id, DateTime changeDate, string? observation, int idTicket, int idStatus, int idUser, CancellationToken cancellationToken = default)
    {
        var historyId = TicketStatusHistoryId.Create(id);
        var existing = await _ticketStatusHistoryRepository.GetByIdAsync(historyId, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"TicketStatusHistory with id '{id}' was not found.");

        var updated = TicketStatusHistory.Create(id, changeDate, observation, idTicket, idStatus, idUser);
        await _ticketStatusHistoryRepository.UpdateAsync(updated, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return updated;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var historyId = TicketStatusHistoryId.Create(id);
        var existing = await _ticketStatusHistoryRepository.GetByIdAsync(historyId, cancellationToken);
        if (existing is null)
            return false;

        await _ticketStatusHistoryRepository.DeleteAsync(historyId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
