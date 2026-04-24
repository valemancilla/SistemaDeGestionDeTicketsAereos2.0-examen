using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.ticket.Application.Services;

public sealed class TicketService : ITicketService
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TicketService(ITicketRepository ticketRepository, IUnitOfWork unitOfWork)
    {
        _ticketRepository = ticketRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Ticket> CreateAsync(string code, DateTime issueDate, int idBooking, int idFare, int idStatus, CancellationToken cancellationToken = default)
    {
        var existing = await _ticketRepository.GetByCodeAsync(code, cancellationToken);
        if (existing is not null)
            throw new InvalidOperationException($"Ticket with code '{code}' already exists.");

        _ = issueDate; // reservado por la interfaz; la emisión usa un solo "ahora" en el agregado
        var entity = Ticket.CreateNew(code, idBooking, idFare, idStatus);
        await _ticketRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public Task<Ticket?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _ticketRepository.GetByIdAsync(TicketId.Create(id), cancellationToken);
    }

    public async Task<IReadOnlyCollection<Ticket>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _ticketRepository.ListAsync(cancellationToken);
    }

    public async Task<Ticket> UpdateAsync(int id, string code, DateTime issueDate, int idBooking, int idFare, int idStatus, CancellationToken cancellationToken = default)
    {
        var ticketId = TicketId.Create(id);
        var existing = await _ticketRepository.GetByIdAsync(ticketId, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"Ticket with id '{id}' was not found.");

        if (issueDate > DateTime.Now)
            throw new ArgumentException("La fecha de emisión no puede ser futura.", nameof(issueDate));

        var updated = Ticket.Create(id, code, issueDate, idBooking, idFare, idStatus);
        await _ticketRepository.UpdateAsync(updated, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return updated;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var ticketId = TicketId.Create(id);
        var existing = await _ticketRepository.GetByIdAsync(ticketId, cancellationToken);
        if (existing is null)
            return false;

        await _ticketRepository.DeleteAsync(ticketId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
