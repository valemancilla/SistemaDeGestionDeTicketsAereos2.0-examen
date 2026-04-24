// Contrato del servicio de aviones: define las operaciones de negocio disponibles para la capa de presentación
// El alias AircraftAggregate evita conflicto con el namespace "Aircraft" y el tipo "Aircraft"
using AircraftAggregate = global::SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Domain.aggregate.Aircraft;

namespace SistemaDeGestionDeTicketsAereos.src.modules.Aircraft.Application.Interfaces;

// Interfaz del servicio de aviones — desacopla la capa UI del servicio concreto
public interface IAircraftService
{
    // Crea un nuevo avión con capacidad, aerolínea y modelo especificados
    Task<AircraftAggregate> CreateAsync(int capacity, int idAirline, int idModel, CancellationToken cancellationToken = default);

    // Busca un avión por su ID, retorna null si no existe
    Task<AircraftAggregate?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    // Retorna todos los aviones registrados en el sistema
    Task<IReadOnlyCollection<AircraftAggregate>> GetAllAsync(CancellationToken cancellationToken = default);

    // Actualiza los datos de un avión existente, lanza excepción si no se encuentra
    Task<AircraftAggregate> UpdateAsync(int id, int capacity, int idAirline, int idModel, CancellationToken cancellationToken = default);

    // Elimina un avión por su ID, retorna false si no existe
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
