// Implementación del servicio de modelos de aeronave: coordina el repositorio y la unidad de trabajo
using SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Application.Services;

// Servicio de aplicación de modelos de aeronave — orquesta sin lógica de dominio propia
public sealed class AircraftModelService : IAircraftModelService
{
    private readonly IAircraftModelRepository _aircraftModelRepository;
    private readonly IUnitOfWork _unitOfWork;

    // Inyección de dependencias: el repositorio y la unidad de trabajo llegan por constructor
    public AircraftModelService(IAircraftModelRepository aircraftModelRepository, IUnitOfWork unitOfWork)
    {
        _aircraftModelRepository = aircraftModelRepository;
        _unitOfWork = unitOfWork;
    }

    // Crea un modelo nuevo y lo persiste inmediatamente
    public async Task<AircraftModel> CreateAsync(string name, int idManufacturer, CancellationToken cancellationToken = default)
    {
        var entity = AircraftModel.CreateNew(name, idManufacturer);
        await _aircraftModelRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    // Busca un modelo por ID delegando directamente al repositorio
    public Task<AircraftModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _aircraftModelRepository.GetByIdAsync(AircraftModelId.Create(id), cancellationToken);
    }

    // Retorna todos los modelos sin filtro
    public async Task<IReadOnlyCollection<AircraftModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _aircraftModelRepository.ListAsync(cancellationToken);
    }

    // Actualiza un modelo verificando que exista, luego recrea el agregado con los nuevos datos
    public async Task<AircraftModel> UpdateAsync(int id, string name, int idManufacturer, CancellationToken cancellationToken = default)
    {
        var modelId = AircraftModelId.Create(id);
        var existing = await _aircraftModelRepository.GetByIdAsync(modelId, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"AircraftModel with id '{id}' was not found.");

        var updated = AircraftModel.Create(id, name, idManufacturer);
        await _aircraftModelRepository.UpdateAsync(updated, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return updated;
    }

    // Elimina un modelo por su ID, retorna false si no existe en lugar de lanzar excepción
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var modelId = AircraftModelId.Create(id);
        var existing = await _aircraftModelRepository.GetByIdAsync(modelId, cancellationToken);
        if (existing is null)
            return false;

        await _aircraftModelRepository.DeleteAsync(modelId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
