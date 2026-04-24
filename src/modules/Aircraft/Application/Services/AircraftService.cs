// Implementación del servicio de aviones: coordina el repositorio y la unidad de trabajo
using AircraftAggregate = global::SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Domain.aggregate.Aircraft;
using SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Application.Services;

// Servicio de aplicación de aviones — orquesta los casos de uso sin lógica de dominio propia
public sealed class AircraftService : IAircraftService
{
    private readonly IAircraftRepository _aircraftRepository;
    private readonly IUnitOfWork _unitOfWork;

    // Inyección de dependencias: el repositorio y la unidad de trabajo llegan por constructor
    public AircraftService(IAircraftRepository aircraftRepository, IUnitOfWork unitOfWork)
    {
        _aircraftRepository = aircraftRepository;
        _unitOfWork = unitOfWork;
    }

    // Crea un avión nuevo y lo persiste inmediatamente
    public async Task<AircraftAggregate> CreateAsync(int capacity, int idAirline, int idModel, CancellationToken cancellationToken = default)
    {
        var entity = AircraftAggregate.CreateNew(capacity, idAirline, idModel);
        await _aircraftRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    // Busca un avión por ID delegando directamente al repositorio
    public Task<AircraftAggregate?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _aircraftRepository.GetByIdAsync(AircraftId.Create(id), cancellationToken);
    }

    // Retorna todos los aviones sin filtro
    public async Task<IReadOnlyCollection<AircraftAggregate>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _aircraftRepository.ListAsync(cancellationToken);
    }

    // Actualiza un avión verificando que exista, luego recrea el agregado con los nuevos datos
    public async Task<AircraftAggregate> UpdateAsync(int id, int capacity, int idAirline, int idModel, CancellationToken cancellationToken = default)
    {
        var aircraftId = AircraftId.Create(id);
        var existing = await _aircraftRepository.GetByIdAsync(aircraftId, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"Aircraft with id '{id}' was not found.");

        var updated = AircraftAggregate.Create(id, capacity, idAirline, idModel);
        await _aircraftRepository.UpdateAsync(updated, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return updated;
    }

    // Elimina un avión por su ID, retorna false si no existe en lugar de lanzar excepción
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var aircraftId = AircraftId.Create(id);
        var existing = await _aircraftRepository.GetByIdAsync(aircraftId, cancellationToken);
        if (existing is null)
            return false;

        await _aircraftRepository.DeleteAsync(aircraftId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
