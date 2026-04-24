using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.employee.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.employee.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.employee.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.employee.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.employee.Infrastructure.Repositories;

public sealed class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _dbContext;

    public EmployeeRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Employee?> GetByIdAsync(EmployeeId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<EmployeeEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdEmployee == id.Value, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<Employee?> GetByPersonIdAsync(int idPerson, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<EmployeeEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdPerson == idPerson, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<Employee>> ListAsync(CancellationToken ct = default)
    {
        var query = _dbContext.Set<EmployeeEntity>().AsNoTracking();
        var entities = await query.OrderBy(x => x.IdEmployee).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task<IReadOnlyList<Employee>> ListByAirlineAsync(int idAirline, CancellationToken ct = default)
    {
        var query = _dbContext.Set<EmployeeEntity>().AsNoTracking();
        query = query.Where(x => x.IdAirline == idAirline);
        var entities = await query.OrderBy(x => x.IdEmployee).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task AddAsync(Employee employee, CancellationToken ct = default)
    {
        var entity = ToEntity(employee);
        await _dbContext.Set<EmployeeEntity>().AddAsync(entity, ct);
    }

    public async Task UpdateAsync(Employee employee, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<EmployeeEntity>().FirstOrDefaultAsync(x => x.IdEmployee == employee.Id.Value, ct);

        if (entity is null)
        {
            throw new KeyNotFoundException($"Employee with id '{employee.Id.Value}' was not found.");
        }

        entity.IdPerson = employee.IdPerson;
        entity.IdAirline = employee.IdAirline;
        entity.IdRole = employee.IdRole;
    }

    public async Task DeleteAsync(EmployeeId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<EmployeeEntity>().FirstOrDefaultAsync(x => x.IdEmployee == id.Value, ct);

        if (entity is null)
        {
            return;
        }

        _dbContext.Set<EmployeeEntity>().Remove(entity);
    }

    private static Employee ToDomain(EmployeeEntity entity)
    {
        return Employee.Create(entity.IdEmployee, entity.IdPerson, entity.IdAirline, entity.IdRole);
    }

    private static EmployeeEntity ToEntity(Employee aggregate)
    {
        return new EmployeeEntity
        {
            IdEmployee = aggregate.Id.Value,
            IdPerson = aggregate.IdPerson,
            IdAirline = aggregate.IdAirline,
            IdRole = aggregate.IdRole
        };
    }
}
