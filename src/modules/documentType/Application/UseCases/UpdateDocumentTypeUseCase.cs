// Caso de uso: actualizar un tipo de documento existente verificando que exista antes de modificarlo
using SistemaDeGestionDeTicketsAereos.src.modules.documentType.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.documentType.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.documentType.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.documentType.Application.UseCases;

public sealed class UpdateDocumentTypeUseCase
{
    private readonly IDocumentTypeRepository _repo;
    public UpdateDocumentTypeUseCase(IDocumentTypeRepository repo) => _repo = repo;

    // Verifica que el tipo exista antes de actualizarlo — recrea el agregado con los nuevos datos
    public async Task<DocumentType> ExecuteAsync(int id, string name, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(DocumentTypeId.Create(id), ct);
        if (existing is null) throw new KeyNotFoundException($"DocumentType with id '{id}' was not found.");
        var updated = DocumentType.Create(id, name);
        await _repo.UpdateAsync(updated, ct);
        return updated;
    }
}
