using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.payment.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.payment.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.payment.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.payment.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.payment.Infrastructure.Repositories;

public sealed class PaymentRepository : IPaymentRepository
{
    private readonly AppDbContext _dbContext;

    public PaymentRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Payment?> GetByIdAsync(PaymentId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<PaymentEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdPayment == id.Value, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<Payment>> ListAsync(CancellationToken ct = default)
    {
        var query = _dbContext.Set<PaymentEntity>().AsNoTracking();
        var entities = await query.OrderBy(x => x.IdPayment).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task AddAsync(Payment payment, CancellationToken ct = default)
    {
        var entity = ToEntity(payment);
        await _dbContext.Set<PaymentEntity>().AddAsync(entity, ct);
    }

    public async Task UpdateAsync(Payment payment, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<PaymentEntity>().FirstOrDefaultAsync(x => x.IdPayment == payment.Id.Value, ct);

        if (entity is null)
        {
            throw new KeyNotFoundException("Payment was not found.");
        }

        var values = ToEntity(payment);
        entity.Amount = values.Amount;
        entity.PaymentDate = values.PaymentDate;
        entity.IdBooking = values.IdBooking;
        entity.IdPaymentMethod = values.IdPaymentMethod;
        entity.IdStatus = values.IdStatus;
        entity.IdTicket = values.IdTicket;
    }

    public async Task DeleteAsync(PaymentId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<PaymentEntity>().FirstOrDefaultAsync(x => x.IdPayment == id.Value, ct);

        if (entity is null)
        {
            return;
        }

        _dbContext.Set<PaymentEntity>().Remove(entity);
    }

    private static Payment ToDomain(PaymentEntity entity)
    {
        return Payment.Create(entity.IdPayment, entity.Amount, entity.PaymentDate, entity.IdBooking, entity.IdPaymentMethod, entity.IdStatus, entity.IdTicket);
    }

    private static PaymentEntity ToEntity(Payment aggregate)
    {
        return new PaymentEntity
        {
            IdPayment = aggregate.Id.Value,
            IdBooking = aggregate.IdBooking,
            IdTicket = aggregate.IdTicket,
            IdPaymentMethod = aggregate.IdPaymentMethod,
            Amount = aggregate.Amount.Value,
            PaymentDate = aggregate.Date.Value,
            IdStatus = aggregate.IdStatus
        };
    }
}
