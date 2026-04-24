using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.customerPhone.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.customerPhone.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.customerPhone.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.customerPhone.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;
using SistemaDeGestionDeTicketsAereos.src.shared.helpers;

namespace SistemaDeGestionDeTicketsAereos.src.modules.customerPhone.Infrastructure.Repositories;

public sealed class CustomerPhoneRepository : ICustomerPhoneRepository
{
    private readonly AppDbContext _dbContext;

    public CustomerPhoneRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CustomerPhone?> GetByIdAsync(CustomerPhoneId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<CustomerPhoneEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdPhone == id.Value, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<CustomerPhone>> ListAsync(CancellationToken ct = default)
    {
        var query = _dbContext.Set<CustomerPhoneEntity>().AsNoTracking();
        var entities = await query.OrderBy(x => x.IdPhone).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task<IReadOnlyList<CustomerPhone>> ListByPersonAsync(int idPerson, CancellationToken ct = default)
    {
        var query = _dbContext.Set<CustomerPhoneEntity>().AsNoTracking();
        query = query.Where(x => x.IdPerson == idPerson);
        var entities = await query.OrderBy(x => x.IdPhone).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task AddAsync(CustomerPhone phone, CancellationToken ct = default)
    {
        var entity = ToEntity(phone);
        await _dbContext.Set<CustomerPhoneEntity>().AddAsync(entity, ct);
    }

    public async Task UpdateAsync(CustomerPhone phone, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<CustomerPhoneEntity>()
            .FirstOrDefaultAsync(x => x.IdPhone == phone.Id.Value, ct);

        if (entity is null)
        {
            throw new KeyNotFoundException("CustomerPhone was not found.");
        }

        var values = ToEntity(phone);
        entity.IdPerson = values.IdPerson;
        entity.Phone = values.Phone;
    }

    public async Task DeleteAsync(CustomerPhoneId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<CustomerPhoneEntity>().FirstOrDefaultAsync(x => x.IdPhone == id.Value, ct);

        if (entity is null)
        {
            return;
        }

        _dbContext.Set<CustomerPhoneEntity>().Remove(entity);
    }

    public async Task<bool> IsPhoneInUseAsync(string phone, int? exceptIdPhone, CancellationToken ct = default)
    {
        var rows = await _dbContext.Set<CustomerPhoneEntity>().AsNoTracking()
            .Select(x => new { x.IdPhone, x.Phone })
            .ToListAsync(ct);
        var n = ContactUniquenessNormalization.NormalizePhone(phone);
        if (n.Length == 0) return false;
        return rows.Any(x => ContactUniquenessNormalization.NormalizePhone(x.Phone) == n
            && (exceptIdPhone is null || x.IdPhone != exceptIdPhone));
    }

    private static CustomerPhone ToDomain(CustomerPhoneEntity entity)
    {
        return CustomerPhone.Create(entity.IdPhone, entity.Phone, entity.IdPerson);
    }

    private static CustomerPhoneEntity ToEntity(CustomerPhone aggregate)
    {
        return new CustomerPhoneEntity
        {
            IdPhone = aggregate.Id.Value,
            IdPerson = aggregate.IdPerson,
            Phone = aggregate.Phone.Value
        };
    }
}
