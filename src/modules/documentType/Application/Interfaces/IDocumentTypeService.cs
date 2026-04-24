// Contrato del servicio de tipos de documento: define las operaciones para la capa de presentación
using SistemaDeGestionDeTicketsAereos.src.modules.documentType.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.documentType.Application.Interfaces;

// Interfaz del servicio de tipos de documento — desacopla la capa UI del servicio concreto
public interface IDocumentTypeService
{
    // Registra un nuevo tipo de documento (DNI, pasaporte, etc.)
    Task<DocumentType> CreateAsync(string name, CancellationToken cancellationToken = default);

    // Busca un tipo de documento por su ID, retorna null si no existe
    Task<DocumentType?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    // Retorna todos los tipos de documento registrados en el sistema
    Task<IReadOnlyCollection<DocumentType>> GetAllAsync(CancellationToken cancellationToken = default);

    // Actualiza el nombre de un tipo de documento existente, lanza excepción si no se encuentra
    Task<DocumentType> UpdateAsync(int id, string name, CancellationToken cancellationToken = default);

    // Elimina un tipo de documento por su ID, retorna false si no existe
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
