using SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Application.Interfaces;

public interface IBoardingPassService
{
    Task<BoardingPass> CreateAsync(
        string code,
        int idTicket,
        int idSeat,
        string gate,
        DateTime boardingTime,
        DateTime createdAt,
        int idStatus,
        string passengerFullName,
        CancellationToken cancellationToken = default);

    Task<BoardingPass?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<BoardingPass?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<BoardingPass?> GetByTicketIdAsync(int idTicket, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<BoardingPass>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<BoardingPass> UpdateAsync(
        int id,
        string code,
        int idTicket,
        int idSeat,
        string gate,
        DateTime boardingTime,
        DateTime createdAt,
        int idStatus,
        string passengerFullName,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
