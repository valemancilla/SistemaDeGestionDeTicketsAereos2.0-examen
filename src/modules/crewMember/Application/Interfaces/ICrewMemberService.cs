// Contrato del servicio de miembros de tripulación: define las operaciones para la capa de presentación
using SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.crewMember.Application.Interfaces;

// Interfaz del servicio de miembros de tripulación — desacopla la capa UI del servicio concreto
public interface ICrewMemberService
{
    // Vincula un empleado a una tripulación con un rol específico (ej. piloto, auxiliar)
    Task<CrewMember> CreateAsync(int idCrew, int idEmployee, int idRole, CancellationToken cancellationToken = default);

    // Busca un miembro de tripulación por su ID, retorna null si no existe
    Task<CrewMember?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    // Retorna todos los miembros de tripulación registrados en el sistema
    Task<IReadOnlyCollection<CrewMember>> GetAllAsync(CancellationToken cancellationToken = default);

    // Actualiza los datos de un miembro existente, lanza excepción si no se encuentra
    Task<CrewMember> UpdateAsync(int id, int idCrew, int idEmployee, int idRole, CancellationToken cancellationToken = default);

    // Elimina un miembro de tripulación por su ID, retorna false si no existe
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
