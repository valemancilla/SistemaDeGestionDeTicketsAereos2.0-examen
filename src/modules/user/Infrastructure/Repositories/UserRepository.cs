// Este repositorio hace el puente entre la base de datos y el dominio de usuario
// Convierte entidades de EF Core en agregados del dominio y viceversa
using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.user.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.user.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.user.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.user.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.user.Infrastructure.Repositories;

// Implementación concreta de IUserRepository usando Entity Framework Core
public sealed class UserRepository : IUserRepository
{
    // Contexto de base de datos inyectado por el sistema de DI
    private readonly AppDbContext _context;
    public UserRepository(AppDbContext context) => _context = context;

    // Busca un usuario por su ID, retorna null si no existe
    public async Task<User?> GetByIdAsync(UserId id, CancellationToken ct = default)
    {
        var e = await _context.Set<UserEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdUser == id.Value, ct);
        return e is null ? null : ToDomain(e);
    }

    // Busca un usuario por su nombre de usuario, útil para el login
    public async Task<User?> GetUserByUsernameAsync(string username, CancellationToken ct = default)
    {
        var e = await _context.Set<UserEntity>().FirstOrDefaultAsync(u => u.Username == username, ct);
        return e is null ? null : ToDomain(e);
    }

    // Retorna todos los usuarios del sistema como una lista de solo lectura
    public async Task<IReadOnlyList<User>> ListAsync(CancellationToken ct = default)
    {
        var list = await _context.Set<UserEntity>().AsNoTracking().ToListAsync(ct);
        return list.Select(ToDomain).ToList();
    }

    // Agrega un nuevo usuario a la base de datos
    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        var e = ToEntity(user);
        await _context.Set<UserEntity>().AddAsync(e, ct);
    }

    // Actualiza los datos de un usuario existente en la base de datos
    public async Task UpdateAsync(User user, CancellationToken ct = default)
    {
        var e = await _context.Set<UserEntity>().FindAsync(new object[] { user.Id.Value }, ct);
        if (e is null) return;
        e.Username = user.Username.Value;
        e.Password = user.Password.Value;
        e.IdUserRole = user.IdUserRole;
        e.IdPerson = user.IdPerson;
        e.Active = user.Active;
    }

    // Elimina un usuario de la base de datos buscándolo por ID
    public async Task DeleteAsync(UserId id, CancellationToken ct = default)
    {
        var e = await _context.Set<UserEntity>().FindAsync(new object[] { id.Value }, ct);
        if (e is null) return;
        _context.Set<UserEntity>().Remove(e);
    }

    // Convierte una entidad de base de datos en un agregado del dominio
    // Usa User.Create para que el dominio controle su propia construcción con Value Objects
    private static User ToDomain(UserEntity e) =>
        User.Create(e.IdUser, e.Username, e.Password, e.IdUserRole, e.IdPerson, e.Active);

    // Convierte un agregado del dominio en una entidad de EF Core lista para guardar
    private static UserEntity ToEntity(User u) => new()
    {
        IdUser = u.Id.Value,
        Username = u.Username.Value,
        Password = u.Password.Value,
        IdUserRole = u.IdUserRole,
        IdPerson = u.IdPerson,
        Active = u.Active
    };
}
