using SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Application.Interfaces;

public interface ITicketStatusHistoryService
{
    Task<TicketStatusHistory> CreateAsync(DateTime changeDate, string? observation, int idTicket, int idStatus, int idUser, CancellationToken cancellationToken = default);

    Task<TicketStatusHistory?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<TicketStatusHistory>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<TicketStatusHistory> UpdateAsync(int id, DateTime changeDate, string? observation, int idTicket, int idStatus, int idUser, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
