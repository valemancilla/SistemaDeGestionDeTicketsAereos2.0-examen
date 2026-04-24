using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Infrastructure.Repositories;

public sealed class CheckInChannelRepository : ICheckInChannelRepository
{
    private readonly AppDbContext _dbContext;

    public CheckInChannelRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CheckInChannel?> GetByIdAsync(CheckInChannelId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<CheckInChannelEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdChannel == id.Value, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<CheckInChannel>> ListAsync(CancellationToken ct = default)
    {
        var query = _dbContext.Set<CheckInChannelEntity>().AsNoTracking();
        var entities = await query.OrderBy(x => x.IdChannel).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task AddAsync(CheckInChannel channel, CancellationToken ct = default)
    {
        var entity = ToEntity(channel);
        await _dbContext.Set<CheckInChannelEntity>().AddAsync(entity, ct);
    }

    public async Task UpdateAsync(CheckInChannel channel, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<CheckInChannelEntity>().FirstOrDefaultAsync(x => x.IdChannel == channel.Id.Value, ct);

        if (entity is null)
        {
            throw new KeyNotFoundException("CheckInChannel was not found.");
        }

        var values = ToEntity(channel);
        entity.ChannelName = values.ChannelName;
    }

    public async Task DeleteAsync(CheckInChannelId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<CheckInChannelEntity>().FirstOrDefaultAsync(x => x.IdChannel == id.Value, ct);

        if (entity is null)
        {
            return;
        }

        _dbContext.Set<CheckInChannelEntity>().Remove(entity);
    }

    private static CheckInChannel ToDomain(CheckInChannelEntity entity)
    {
        return CheckInChannel.Create(entity.IdChannel, entity.ChannelName);
    }

    private static CheckInChannelEntity ToEntity(CheckInChannel aggregate)
    {
        return new CheckInChannelEntity
        {
            IdChannel = aggregate.Id.Value,
            ChannelName = aggregate.Name.Value
        };
    }
}
