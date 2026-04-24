using SistemaDeGestionDeTicketsAereos.src.modules.user.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.user.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.user.Application.UseCases;

public sealed class LoginUseCase
{
    private readonly IUserRepository _repo;
    public LoginUseCase(IUserRepository repo) => _repo = repo;

    public async Task<User> ExecuteAsync(string username, string password, CancellationToken ct = default)
    {
        var user = await _repo.GetUserByUsernameAsync(username, ct);
        if (user is null)
            throw new UnauthorizedAccessException("No existe un usuario con ese nombre. Verifica el nombre o regístrate.");
        if (user.Password.Value != password)
            throw new UnauthorizedAccessException("La contraseña no es correcta.");
        if (!user.Active)
            throw new UnauthorizedAccessException("La cuenta está desactivada. Contacta al administrador.");
        return user;
    }
}
