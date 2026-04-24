// Contrato del repositorio de miembros de tripulación: define las operaciones de persistencia disponibles
using SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Domain.Repositories;

// Interfaz que define las operaciones CRUD del repositorio de miembros de tripulación
public interface ICrewMemberRepository
{
    // Busca un miembro de tripulación por su ID
    Task<CrewMember?> GetByIdAsync(CrewMemberId id, CancellationToken ct = default);

    // Retorna todos los miembros de tripulación del sistema
    Task<IReadOnlyList<CrewMember>> ListAsync(CancellationToken ct = default);

    // Retorna todos los miembros que pertenecen a un grupo de tripulación específico
    Task<IReadOnlyList<CrewMember>> ListByCrewAsync(int idCrew, CancellationToken ct = default);

    // Agrega un nuevo miembro de tripulación al sistema
    Task AddAsync(CrewMember crewMember, CancellationToken ct = default);

    // Actualiza los datos de un miembro de tripulación existente
    Task UpdateAsync(CrewMember crewMember, CancellationToken ct = default);

    // Elimina un miembro de tripulación por su ID
    Task DeleteAsync(CrewMemberId id, CancellationToken ct = default);
}
