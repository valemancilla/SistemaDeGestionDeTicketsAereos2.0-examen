using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Infrastructure.Repositories;

public sealed class ManufacturerRepository : IManufacturerRepository
{
    private readonly AppDbContext _dbContext;

    public ManufacturerRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Manufacturer?> GetByIdAsync(ManufacturerId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<ManufacturerEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdManufacturer == id.Value, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<Manufacturer?> GetByNameAsync(string name, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<ManufacturerEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == name, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<Manufacturer>> ListAsync(CancellationToken ct = default)
    {
        var query = _dbContext.Set<ManufacturerEntity>().AsNoTracking();
        var entities = await query.OrderBy(x => x.IdManufacturer).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task AddAsync(Manufacturer manufacturer, CancellationToken ct = default)
    {
        var entity = ToEntity(manufacturer);
        await _dbContext.Set<ManufacturerEntity>().AddAsync(entity, ct);
    }

    public async Task UpdateAsync(Manufacturer manufacturer, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<ManufacturerEntity>().FirstOrDefaultAsync(x => x.IdManufacturer == manufacturer.Id.Value, ct);

        if (entity is null)
        {
            throw new KeyNotFoundException("Manufacturer was not found.");
        }

        var values = ToEntity(manufacturer);
        entity.Name = values.Name;
    }

    public async Task DeleteAsync(ManufacturerId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<ManufacturerEntity>().FirstOrDefaultAsync(x => x.IdManufacturer == id.Value, ct);

        if (entity is null)
        {
            return;
        }

        // Si existen modelos asociados a este fabricante, deben eliminarse primero (FK RESTRICT).
        // Esto a su vez elimina aeronaves y toda su rama (vuelos, reservas, asientos, etc.) mediante el repositorio de Aircraft.
        var modelIds = await _dbContext.Set<AircraftModelEntity>()
            .AsNoTracking()
            .Where(m => m.IdManufacturer == id.Value)
            .Select(m => m.IdModel)
            .ToListAsync(ct);

        if (modelIds.Count > 0)
        {
            var modelRepo = new AircraftModelRepository(_dbContext);
            foreach (var modelId in modelIds)
            {
                await modelRepo.DeleteAsync(AircraftModelId.Create(modelId), ct);
            }
        }

        _dbContext.Set<ManufacturerEntity>().Remove(entity);
    }

    private static Manufacturer ToDomain(ManufacturerEntity entity)
    {
        return Manufacturer.Create(entity.IdManufacturer, entity.Name);
    }

    private static ManufacturerEntity ToEntity(Manufacturer aggregate)
    {
        return new ManufacturerEntity
        {
            IdManufacturer = aggregate.Id.Value,
            Name = aggregate.Name.Value
        };
    }
}
