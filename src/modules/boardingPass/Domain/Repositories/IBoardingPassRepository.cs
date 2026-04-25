using SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Domain.Repositories;

public interface IBoardingPassRepository
{
    Task<BoardingPass?> GetByIdAsync(BoardingPassId id, CancellationToken ct = default);
    Task<BoardingPass?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<BoardingPass?> GetByTicketIdAsync(int idTicket, CancellationToken ct = default);
    Task<IReadOnlyList<BoardingPass>> ListAsync(CancellationToken ct = default);
    Task AddAsync(BoardingPass boardingPass, CancellationToken ct = default);
    Task UpdateAsync(BoardingPass boardingPass, CancellationToken ct = default);
    Task DeleteAsync(BoardingPassId id, CancellationToken ct = default);
}

