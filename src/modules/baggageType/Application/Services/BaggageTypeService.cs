// Implementación del servicio de tipos de equipaje: coordina el repositorio y la unidad de trabajo
using SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Application.Services;

// Servicio de aplicación de tipos de equipaje — orquesta sin lógica de dominio propia
public sealed class BaggageTypeService : IBaggageTypeService
{
    private readonly IBaggageTypeRepository _baggageTypeRepository;
    private readonly IUnitOfWork _unitOfWork;

    // Inyección de dependencias: el repositorio y la unidad de trabajo llegan por constructor
    public BaggageTypeService(IBaggageTypeRepository baggageTypeRepository, IUnitOfWork unitOfWork)
    {
        _baggageTypeRepository = baggageTypeRepository;
        _unitOfWork = unitOfWork;
    }

    // Crea un tipo de equipaje nuevo y lo persiste inmediatamente
    public async Task<BaggageType> CreateAsync(string name, CancellationToken cancellationToken = default)
    {
        var entity = BaggageType.CreateNew(name);
        await _baggageTypeRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    // Busca un tipo por ID delegando directamente al repositorio
    public Task<BaggageType?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _baggageTypeRepository.GetByIdAsync(BaggageTypeId.Create(id), cancellationToken);
    }

    // Retorna todos los tipos de equipaje sin filtro
    public async Task<IReadOnlyCollection<BaggageType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _baggageTypeRepository.ListAsync(cancellationToken);
    }

    // Actualiza un tipo verificando que exista, luego recrea el agregado con los nuevos datos
    public async Task<BaggageType> UpdateAsync(int id, string name, CancellationToken cancellationToken = default)
    {
        var baggageTypeId = BaggageTypeId.Create(id);
        var existing = await _baggageTypeRepository.GetByIdAsync(baggageTypeId, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"BaggageType with id '{id}' was not found.");

        var updated = BaggageType.Create(id, name);
        await _baggageTypeRepository.UpdateAsync(updated, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return updated;
    }

    // Elimina un tipo por su ID, retorna false si no existe en lugar de lanzar excepción
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var baggageTypeId = BaggageTypeId.Create(id);
        var existing = await _baggageTypeRepository.GetByIdAsync(baggageTypeId, cancellationToken);
        if (existing is null)
            return false;

        await _baggageTypeRepository.DeleteAsync(baggageTypeId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
