// Contrato del repositorio de tipos de documento: define las operaciones de persistencia disponibles
using SistemaDeGestionDeTicketsAereos.src.modules.documentType.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.documentType.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.documentType.Domain.Repositories;

// Interfaz que define las operaciones CRUD del repositorio de tipos de documento
public interface IDocumentTypeRepository
{
    // Busca un tipo de documento por su ID
    Task<DocumentType?> GetByIdAsync(DocumentTypeId id, CancellationToken ct = default);

    // Retorna todos los tipos de documento registrados
    Task<IReadOnlyList<DocumentType>> ListAsync(CancellationToken ct = default);

    // Agrega un nuevo tipo de documento al sistema
    Task AddAsync(DocumentType documentType, CancellationToken ct = default);

    // Actualiza los datos de un tipo de documento existente
    Task UpdateAsync(DocumentType documentType, CancellationToken ct = default);

    // Elimina un tipo de documento del sistema por su ID
    Task DeleteAsync(DocumentTypeId id, CancellationToken ct = default);
}
