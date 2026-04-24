using SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Application.Interfaces;

public interface ISeatClassService
{
    Task<SeatClass> CreateAsync(string name, CancellationToken cancellationToken = default);

    Task<SeatClass?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SeatClass>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<SeatClass> UpdateAsync(int id, string name, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
