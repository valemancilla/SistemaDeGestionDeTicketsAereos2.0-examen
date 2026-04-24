using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Infrastructure.Repositories;

public sealed class PaymentMethodRepository : IPaymentMethodRepository
{
    private readonly AppDbContext _dbContext;

    public PaymentMethodRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaymentMethod?> GetByIdAsync(PaymentMethodId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<PaymentMethodEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdPaymentMethod == id.Value, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<PaymentMethod>> ListAsync(CancellationToken ct = default)
    {
        var query = _dbContext.Set<PaymentMethodEntity>().AsNoTracking();
        var entities = await query.OrderBy(x => x.IdPaymentMethod).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task AddAsync(PaymentMethod paymentMethod, CancellationToken ct = default)
    {
        var entity = ToEntity(paymentMethod);
        await _dbContext.Set<PaymentMethodEntity>().AddAsync(entity, ct);
    }

    public async Task UpdateAsync(PaymentMethod paymentMethod, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<PaymentMethodEntity>().FirstOrDefaultAsync(x => x.IdPaymentMethod == paymentMethod.Id.Value, ct);

        if (entity is null)
        {
            throw new KeyNotFoundException("PaymentMethod was not found.");
        }

        var values = ToEntity(paymentMethod);
        entity.MethodName = values.MethodName;
    }

    public async Task DeleteAsync(PaymentMethodId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<PaymentMethodEntity>().FirstOrDefaultAsync(x => x.IdPaymentMethod == id.Value, ct);

        if (entity is null)
        {
            return;
        }

        _dbContext.Set<PaymentMethodEntity>().Remove(entity);
    }

    private static PaymentMethod ToDomain(PaymentMethodEntity entity)
    {
        return PaymentMethod.Create(entity.IdPaymentMethod, entity.MethodName);
    }

    private static PaymentMethodEntity ToEntity(PaymentMethod aggregate)
    {
        return new PaymentMethodEntity
        {
            IdPaymentMethod = aggregate.Id.Value,
            MethodName = aggregate.Name.Value
        };
    }
}
