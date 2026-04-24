using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.customerEmail.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.customerEmail.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.customerEmail.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.customerEmail.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;
using SistemaDeGestionDeTicketsAereos.src.shared.helpers;

namespace SistemaDeGestionDeTicketsAereos.src.modules.customerEmail.Infrastructure.Repositories;

public sealed class CustomerEmailRepository : ICustomerEmailRepository
{
    private readonly AppDbContext _dbContext;

    public CustomerEmailRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CustomerEmail?> GetByIdAsync(CustomerEmailId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<CustomerEmailEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdEmail == id.Value, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<CustomerEmail>> ListAsync(CancellationToken ct = default)
    {
        var query = _dbContext.Set<CustomerEmailEntity>().AsNoTracking();
        var entities = await query.OrderBy(x => x.IdEmail).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task<IReadOnlyList<CustomerEmail>> ListByPersonAsync(int idPerson, CancellationToken ct = default)
    {
        var query = _dbContext.Set<CustomerEmailEntity>().AsNoTracking();
        query = query.Where(x => x.IdPerson == idPerson);
        var entities = await query.OrderBy(x => x.IdEmail).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task AddAsync(CustomerEmail email, CancellationToken ct = default)
    {
        var entity = ToEntity(email);
        await _dbContext.Set<CustomerEmailEntity>().AddAsync(entity, ct);
    }

    public async Task UpdateAsync(CustomerEmail email, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<CustomerEmailEntity>()
            .FirstOrDefaultAsync(x => x.IdEmail == email.Id.Value, ct);

        if (entity is null)
        {
            throw new KeyNotFoundException("CustomerEmail was not found.");
        }

        var values = ToEntity(email);
        entity.IdPerson = values.IdPerson;
        entity.Email = values.Email;
    }

    public async Task DeleteAsync(CustomerEmailId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<CustomerEmailEntity>().FirstOrDefaultAsync(x => x.IdEmail == id.Value, ct);

        if (entity is null)
        {
            return;
        }

        _dbContext.Set<CustomerEmailEntity>().Remove(entity);
    }

    public async Task<bool> IsEmailInUseAsync(string email, int? exceptIdEmail, CancellationToken ct = default)
    {
        var rows = await _dbContext.Set<CustomerEmailEntity>().AsNoTracking()
            .Select(x => new { x.IdEmail, x.Email })
            .ToListAsync(ct);
        var n = ContactUniquenessNormalization.NormalizeEmail(email);
        if (n.Length == 0) return false;
        return rows.Any(x => ContactUniquenessNormalization.NormalizeEmail(x.Email) == n
            && (exceptIdEmail is null || x.IdEmail != exceptIdEmail));
    }

    private static CustomerEmail ToDomain(CustomerEmailEntity entity)
    {
        return CustomerEmail.Create(entity.IdEmail, entity.Email, entity.IdPerson);
    }

    private static CustomerEmailEntity ToEntity(CustomerEmail aggregate)
    {
        return new CustomerEmailEntity
        {
            IdEmail = aggregate.Id.Value,
            IdPerson = aggregate.IdPerson,
            Email = aggregate.Email.Value
        };
    }
}
