using SistemaDeGestionDeTicketsAereos.src.modules.gender.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.gender.Application.Interfaces;

public interface IGenderService
{
    Task<Gender> CreateAsync(string description, CancellationToken cancellationToken = default);

    Task<Gender?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Gender>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Gender> UpdateAsync(int id, string description, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
