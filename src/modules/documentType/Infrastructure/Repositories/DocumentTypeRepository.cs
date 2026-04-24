using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.documentType.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.documentType.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.documentType.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.documentType.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.modules.documentType.Infrastructure.Repositories;

public sealed class DocumentTypeRepository : IDocumentTypeRepository
{
    private readonly AppDbContext _dbContext;

    public DocumentTypeRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DocumentType?> GetByIdAsync(DocumentTypeId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<DocumentTypeEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdDocumentType == id.Value, ct);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyList<DocumentType>> ListAsync(CancellationToken ct = default)
    {
        var query = _dbContext.Set<DocumentTypeEntity>().AsNoTracking();
        var entities = await query.OrderBy(x => x.IdDocumentType).ToListAsync(ct);
        return entities.Select(ToDomain).ToList();
    }

    public async Task AddAsync(DocumentType documentType, CancellationToken ct = default)
    {
        var entity = ToEntity(documentType);
        await _dbContext.Set<DocumentTypeEntity>().AddAsync(entity, ct);
    }

    public async Task UpdateAsync(DocumentType documentType, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<DocumentTypeEntity>().FirstOrDefaultAsync(x => x.IdDocumentType == documentType.Id.Value, ct);

        if (entity is null)
        {
            throw new KeyNotFoundException("DocumentType was not found.");
        }

        var values = ToEntity(documentType);
        entity.Name = values.Name;
    }

    public async Task DeleteAsync(DocumentTypeId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Set<DocumentTypeEntity>().FirstOrDefaultAsync(x => x.IdDocumentType == id.Value, ct);

        if (entity is null)
        {
            return;
        }

        _dbContext.Set<DocumentTypeEntity>().Remove(entity);
    }

    private static DocumentType ToDomain(DocumentTypeEntity entity)
    {
        return DocumentType.Create(entity.IdDocumentType, entity.Name);
    }

    private static DocumentTypeEntity ToEntity(DocumentType aggregate)
    {
        return new DocumentTypeEntity
        {
            IdDocumentType = aggregate.Id.Value,
            Name = aggregate.Name.Value
        };
    }
}
