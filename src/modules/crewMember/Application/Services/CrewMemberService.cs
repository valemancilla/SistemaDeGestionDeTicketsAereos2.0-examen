// Implementación del servicio de miembros de tripulación: coordina el repositorio y la unidad de trabajo
using SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Application.Services;

// Servicio de aplicación de miembros de tripulación — orquesta sin lógica de dominio propia
public sealed class CrewMemberService : ICrewMemberService
{
    private readonly ICrewMemberRepository _crewMemberRepository;
    private readonly IUnitOfWork _unitOfWork;

    // Inyección de dependencias: el repositorio y la unidad de trabajo llegan por constructor
    public CrewMemberService(ICrewMemberRepository crewMemberRepository, IUnitOfWork unitOfWork)
    {
        _crewMemberRepository = crewMemberRepository;
        _unitOfWork = unitOfWork;
    }

    // Vincula el empleado a la tripulación con su rol y persiste inmediatamente
    public async Task<CrewMember> CreateAsync(int idCrew, int idEmployee, int idRole, CancellationToken cancellationToken = default)
    {
        var entity = CrewMember.CreateNew(idCrew, idEmployee, idRole);
        await _crewMemberRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    // Busca un miembro por ID delegando directamente al repositorio
    public Task<CrewMember?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _crewMemberRepository.GetByIdAsync(CrewMemberId.Create(id), cancellationToken);
    }

    // Retorna todos los miembros de tripulación sin filtro
    public async Task<IReadOnlyCollection<CrewMember>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _crewMemberRepository.ListAsync(cancellationToken);
    }

    // Actualiza un miembro verificando que exista, luego recrea el agregado con los nuevos datos
    public async Task<CrewMember> UpdateAsync(int id, int idCrew, int idEmployee, int idRole, CancellationToken cancellationToken = default)
    {
        var crewMemberId = CrewMemberId.Create(id);
        var existing = await _crewMemberRepository.GetByIdAsync(crewMemberId, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"CrewMember with id '{id}' was not found.");

        var updated = CrewMember.Create(id, idCrew, idEmployee, idRole);
        await _crewMemberRepository.UpdateAsync(updated, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return updated;
    }

    // Elimina un miembro por su ID, retorna false si no existe en lugar de lanzar excepción
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var crewMemberId = CrewMemberId.Create(id);
        var existing = await _crewMemberRepository.GetByIdAsync(crewMemberId, cancellationToken);
        if (existing is null)
            return false;

        await _crewMemberRepository.DeleteAsync(crewMemberId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
