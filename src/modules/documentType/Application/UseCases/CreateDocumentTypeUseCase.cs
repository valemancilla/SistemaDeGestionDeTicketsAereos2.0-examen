// Caso de uso: registrar un nuevo tipo de documento (DNI, pasaporte, licencia, etc.)
using SistemaDeGestionDeTicketsAereos.src.modules.documentType.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.documentType.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.documentType.Application.UseCases;

public sealed class CreateDocumentTypeUseCase
{
    private readonly IDocumentTypeRepository _repo;
    public CreateDocumentTypeUseCase(IDocumentTypeRepository repo) => _repo = repo;

    // La validación del nombre la hace el agregado
    public async Task<DocumentType> ExecuteAsync(string name, CancellationToken ct = default)
    {
        var entity = DocumentType.CreateNew(name);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}
