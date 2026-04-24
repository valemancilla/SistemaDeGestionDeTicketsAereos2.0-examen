using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.ticket.Application.Interfaces;

public interface ITicketService
{
    Task<Ticket> CreateAsync(string code, DateTime issueDate, int idBooking, int idFare, int idStatus, CancellationToken cancellationToken = default);

    Task<Ticket?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Ticket>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Ticket> UpdateAsync(int id, string code, DateTime issueDate, int idBooking, int idFare, int idStatus, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
