// Caso de uso: eliminar un tipo de documento por su ID
using SistemaDeGestionDeTicketsAereos.src.modules.documentType.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.documentType.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.documentType.Application.UseCases;

public sealed class DeleteDocumentTypeUseCase
{
    private readonly IDocumentTypeRepository _repo;
    public DeleteDocumentTypeUseCase(IDocumentTypeRepository repo) => _repo = repo;

    // Retorna false si no existe — evita lanzar excepción por un recurso que ya no está
    public async Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(DocumentTypeId.Create(id), ct);
        if (existing is null) return false;
        await _repo.DeleteAsync(DocumentTypeId.Create(id), ct);
        return true;
    }
}
