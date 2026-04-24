using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.customer.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.customer.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.customer.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.customer.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.customer.Infrastructure.Repositories;

public sealed class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _dbContext;

    public CustomerRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Customer?> GetByIdAsync(CustomerId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<CustomerEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdCustomer == id.Value, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<Customer?> GetByPersonIdAsync(int idPerson, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<CustomerEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdPerson == idPerson, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<Customer>> ListAsync(CancellationToken ct = default)
    {
        var query = _dbContext.Set<CustomerEntity>().AsNoTracking();
        var entities = await query.OrderBy(x => x.IdCustomer).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task AddAsync(Customer customer, CancellationToken ct = default)
    {
        var entity = ToEntity(customer);
        await _dbContext.Set<CustomerEntity>().AddAsync(entity, ct);
    }

    public async Task UpdateAsync(Customer customer, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<CustomerEntity>()
            .FirstOrDefaultAsync(x => x.IdCustomer == customer.Id.Value, ct);

        if (entity is null)
        {
            throw new KeyNotFoundException("Customer was not found.");
        }

        var values = ToEntity(customer);
        entity.IdPerson = values.IdPerson;
        entity.Active = values.Active;
        entity.RegistrationDate = values.RegistrationDate;
    }

    public async Task DeleteAsync(CustomerId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<CustomerEntity>().FirstOrDefaultAsync(x => x.IdCustomer == id.Value, ct);

        if (entity is null)
        {
            return;
        }

        _dbContext.Set<CustomerEntity>().Remove(entity);
    }

    private static Customer ToDomain(CustomerEntity entity)
    {
        return Customer.Create(entity.IdCustomer, entity.RegistrationDate, entity.IdPerson, entity.Active);
    }

    private static CustomerEntity ToEntity(Customer aggregate)
    {
        return new CustomerEntity
        {
            IdCustomer = aggregate.Id.Value,
            IdPerson = aggregate.IdPerson,
            Active = aggregate.Active,
            RegistrationDate = aggregate.RegistrationDate.Value
        };
    }
}
