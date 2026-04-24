// Caso de uso: buscar un tipo de documento por su ID, lanza excepción si no existe
using SistemaDeGestionDeTicketsAereos.src.modules.documentType.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.documentType.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.documentType.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.documentType.Application.UseCases;

public sealed class GetDocumentTypeByIdUseCase
{
    private readonly IDocumentTypeRepository _repo;
    public GetDocumentTypeByIdUseCase(IDocumentTypeRepository repo) => _repo = repo;

    // La excepción permite que el controlador o la UI devuelvan un 404 claro al usuario
    public async Task<DocumentType> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(DocumentTypeId.Create(id), ct);
        if (entity is null) throw new KeyNotFoundException($"DocumentType with id '{id}' was not found.");
        return entity;
    }
}
