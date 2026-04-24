// Caso de uso: obtener todos los tipos de documento registrados en el sistema
using SistemaDeGestionDeTicketsAereos.src.modules.documentType.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.documentType.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.documentType.Application.UseCases;

public sealed class GetAllDocumentTypesUseCase
{
    private readonly IDocumentTypeRepository _repo;
    public GetAllDocumentTypesUseCase(IDocumentTypeRepository repo) => _repo = repo;

    // Delega directamente al repositorio sin filtros adicionales
    public async Task<IReadOnlyList<DocumentType>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.ListAsync(ct);
}
