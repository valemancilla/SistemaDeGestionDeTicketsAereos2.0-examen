using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Infrastructure.Repositories;

public sealed class AircraftModelRepository : IAircraftModelRepository
{
    private readonly AppDbContext _dbContext;

    public AircraftModelRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AircraftModel?> GetByIdAsync(AircraftModelId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<AircraftModelEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdModel == id.Value, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<AircraftModel>> ListAsync(CancellationToken ct = default)
    {
        var query = _dbContext.Set<AircraftModelEntity>().AsNoTracking();
        var entities = await query.OrderBy(x => x.IdModel).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task<IReadOnlyList<AircraftModel>> ListByManufacturerAsync(int idManufacturer, CancellationToken ct = default)
    {
        var query = _dbContext.Set<AircraftModelEntity>().AsNoTracking();
        query = query.Where(x => x.IdManufacturer == idManufacturer);
        var entities = await query.OrderBy(x => x.IdModel).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task AddAsync(AircraftModel aircraftModel, CancellationToken ct = default)
    {
        var entity = ToEntity(aircraftModel);
        await _dbContext.Set<AircraftModelEntity>().AddAsync(entity, ct);
    }

    public async Task UpdateAsync(AircraftModel aircraftModel, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<AircraftModelEntity>().FirstOrDefaultAsync(x => x.IdModel == aircraftModel.Id.Value, ct);

        if (entity is null)
        {
            throw new KeyNotFoundException("AircraftModel was not found.");
        }

        var values = ToEntity(aircraftModel);
        entity.Model = values.Model;
    }

    public async Task DeleteAsync(AircraftModelId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<AircraftModelEntity>().FirstOrDefaultAsync(x => x.IdModel == id.Value, ct);

        if (entity is null)
        {
            return;
        }

        // Si existen aeronaves usando este modelo, hay que eliminarlas primero (FK RESTRICT).
        var aircraftIds = await _dbContext.Set<AircraftEntity>()
            .AsNoTracking()
            .Where(a => a.IdModel == id.Value)
            .Select(a => a.IdAircraft)
            .ToListAsync(ct);

        if (aircraftIds.Count > 0)
        {
            var aircraftRepo = new AircraftRepository(_dbContext);
            foreach (var aircraftId in aircraftIds)
            {
                await aircraftRepo.DeleteAsync(AircraftId.Create(aircraftId), ct);
            }
        }

        _dbContext.Set<AircraftModelEntity>().Remove(entity);
    }

    private static AircraftModel ToDomain(AircraftModelEntity entity)
    {
        return AircraftModel.Create(entity.IdModel, entity.Model, entity.IdManufacturer);
    }

    private static AircraftModelEntity ToEntity(AircraftModel aggregate)
    {
        return new AircraftModelEntity
        {
            IdModel = aggregate.Id.Value,
            IdManufacturer = aggregate.IdManufacturer,
            Model = aggregate.Name.Value
        };
    }
}
