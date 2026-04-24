using SistemaDeGestionDeTicketsAereos.src.modules.user.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.user.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.user.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.user.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.user.Application.Services;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<User> CreateAsync(string username, string password, int idUserRole, int? idPerson, bool active, CancellationToken cancellationToken = default)
    {
        var existing = await _userRepository.GetUserByUsernameAsync(username, cancellationToken);
        if (existing is not null)
            throw new InvalidOperationException($"User with username '{username}' already exists.");

        var entity = User.CreateNew(username, password, idUserRole, idPerson, active);
        await _userRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _userRepository.GetByIdAsync(UserId.Create(id), cancellationToken);
    }

    public async Task<IReadOnlyCollection<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _userRepository.ListAsync(cancellationToken);
    }

    public async Task<User> UpdateAsync(int id, string username, string password, int idUserRole, int? idPerson, bool active, CancellationToken cancellationToken = default)
    {
        var userId = UserId.Create(id);
        var existing = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"User with id '{id}' was not found.");

        var updated = User.Create(id, username, password, idUserRole, idPerson, active);
        await _userRepository.UpdateAsync(updated, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return updated;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var userId = UserId.Create(id);
        var existing = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (existing is null)
            return false;

        await _userRepository.DeleteAsync(userId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<User> LoginAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetUserByUsernameAsync(username, cancellationToken);
        if (user is null || user.Password.Value != password || !user.Active)
            throw new UnauthorizedAccessException("Invalid credentials or inactive user.");

        return user;
    }
}
