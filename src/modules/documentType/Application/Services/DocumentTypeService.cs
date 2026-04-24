// Implementación del servicio de tipos de documento: coordina el repositorio y la unidad de trabajo
using SistemaDeGestionDeTicketsAereos.src.modules.documentType.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.documentType.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.documentType.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.documentType.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.documentType.Application.Services;

// Servicio de aplicación de tipos de documento — orquesta sin lógica de dominio propia
public sealed class DocumentTypeService : IDocumentTypeService
{
    private readonly IDocumentTypeRepository _documentTypeRepository;
    private readonly IUnitOfWork _unitOfWork;

    // Inyección de dependencias: el repositorio y la unidad de trabajo llegan por constructor
    public DocumentTypeService(IDocumentTypeRepository documentTypeRepository, IUnitOfWork unitOfWork)
    {
        _documentTypeRepository = documentTypeRepository;
        _unitOfWork = unitOfWork;
    }

    // Crea el tipo de documento y lo persiste inmediatamente
    public async Task<DocumentType> CreateAsync(string name, CancellationToken cancellationToken = default)
    {
        var entity = DocumentType.CreateNew(name);
        await _documentTypeRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    // Busca un tipo de documento por ID delegando directamente al repositorio
    public Task<DocumentType?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _documentTypeRepository.GetByIdAsync(DocumentTypeId.Create(id), cancellationToken);
    }

    // Retorna todos los tipos de documento sin filtro
    public async Task<IReadOnlyCollection<DocumentType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _documentTypeRepository.ListAsync(cancellationToken);
    }

    // Actualiza un tipo de documento verificando que exista, luego recrea el agregado con los nuevos datos
    public async Task<DocumentType> UpdateAsync(int id, string name, CancellationToken cancellationToken = default)
    {
        var documentTypeId = DocumentTypeId.Create(id);
        var existing = await _documentTypeRepository.GetByIdAsync(documentTypeId, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"DocumentType with id '{id}' was not found.");

        var updated = DocumentType.Create(id, name);
        await _documentTypeRepository.UpdateAsync(updated, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return updated;
    }

    // Elimina un tipo de documento por su ID, retorna false si no existe en lugar de lanzar excepción
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var documentTypeId = DocumentTypeId.Create(id);
        var existing = await _documentTypeRepository.GetByIdAsync(documentTypeId, cancellationToken);
        if (existing is null)
            return false;

        await _documentTypeRepository.DeleteAsync(documentTypeId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
