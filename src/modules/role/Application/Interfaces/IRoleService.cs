using SistemaDeGestionDeTicketsAereos.src.modules.role.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.role.Application.Interfaces;

public interface IRoleService
{
    Task<Role> CreateAsync(string name, CancellationToken cancellationToken = default);

    Task<Role?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Role>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Role> UpdateAsync(int id, string name, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
