// Implementación del servicio de tripulaciones: coordina el repositorio y la unidad de trabajo
using SistemaDeGestionDeTicketsAereos.src.modules.crew.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.crew.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.crew.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.crew.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.crew.Application.Services;

// Servicio de aplicación de tripulaciones — orquesta sin lógica de dominio propia
public sealed class CrewService : ICrewService
{
    private readonly ICrewRepository _crewRepository;
    private readonly IUnitOfWork _unitOfWork;

    // Inyección de dependencias: el repositorio y la unidad de trabajo llegan por constructor
    public CrewService(ICrewRepository crewRepository, IUnitOfWork unitOfWork)
    {
        _crewRepository = crewRepository;
        _unitOfWork = unitOfWork;
    }

    // Crea la tripulación y la persiste inmediatamente
    public async Task<Crew> CreateAsync(string groupName, CancellationToken cancellationToken = default)
    {
        var entity = Crew.CreateNew(groupName);
        await _crewRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    // Busca una tripulación por ID delegando directamente al repositorio
    public Task<Crew?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _crewRepository.GetByIdAsync(CrewId.Create(id), cancellationToken);
    }

    // Retorna todas las tripulaciones sin filtro
    public async Task<IReadOnlyCollection<Crew>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _crewRepository.ListAsync(cancellationToken);
    }

    // Actualiza una tripulación verificando que exista, luego recrea el agregado con los nuevos datos
    public async Task<Crew> UpdateAsync(int id, string groupName, CancellationToken cancellationToken = default)
    {
        var crewId = CrewId.Create(id);
        var existing = await _crewRepository.GetByIdAsync(crewId, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"Crew with id '{id}' was not found.");

        var updated = Crew.Create(id, groupName);
        await _crewRepository.UpdateAsync(updated, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return updated;
    }

    // Elimina una tripulación por su ID, retorna false si no existe en lugar de lanzar excepción
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var crewId = CrewId.Create(id);
        var existing = await _crewRepository.GetByIdAsync(crewId, cancellationToken);
        if (existing is null)
            return false;

        await _crewRepository.DeleteAsync(crewId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
