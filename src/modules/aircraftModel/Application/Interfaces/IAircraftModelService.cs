// Contrato del servicio de modelos de aeronave: define las operaciones disponibles para la capa de presentación
using SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Application.Interfaces;

// Interfaz del servicio de modelos de aeronave — desacopla la capa UI del servicio concreto
public interface IAircraftModelService
{
    // Crea un nuevo modelo de aeronave (ej: Boeing 737) asociado a un fabricante
    Task<AircraftModel> CreateAsync(string name, int idManufacturer, CancellationToken cancellationToken = default);

    // Busca un modelo por su ID, retorna null si no existe
    Task<AircraftModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    // Retorna todos los modelos de aeronave registrados
    Task<IReadOnlyCollection<AircraftModel>> GetAllAsync(CancellationToken cancellationToken = default);

    // Actualiza los datos de un modelo existente, lanza excepción si no se encuentra
    Task<AircraftModel> UpdateAsync(int id, string name, int idManufacturer, CancellationToken cancellationToken = default);

    // Elimina un modelo por su ID, retorna false si no existe
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
