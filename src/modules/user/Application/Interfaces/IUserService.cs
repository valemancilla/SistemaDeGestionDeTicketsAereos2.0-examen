using SistemaDeGestionDeTicketsAereos.src.modules.user.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.user.Application.Interfaces;

public interface IUserService
{
    Task<User> CreateAsync(string username, string password, int idUserRole, int? idPerson, bool active, CancellationToken cancellationToken = default);

    Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<User>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<User> UpdateAsync(int id, string username, string password, int idUserRole, int? idPerson, bool active, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<User> LoginAsync(string username, string password, CancellationToken cancellationToken = default);
}
