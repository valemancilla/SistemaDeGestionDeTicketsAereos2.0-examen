// Contrato del repositorio de modelos de aeronave: define las operaciones de persistencia disponibles
using SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Domain.Repositories;

// Interfaz que define las operaciones CRUD del repositorio de modelos de aeronave
public interface IAircraftModelRepository
{
    // Busca un modelo por su ID
    Task<AircraftModel?> GetByIdAsync(AircraftModelId id, CancellationToken ct = default);

    // Busca un modelo por su nombre (útil para verificar unicidad)
    Task<AircraftModel?> GetByNameAsync(string name, CancellationToken ct = default);

    // Retorna todos los modelos de aeronave registrados
    Task<IReadOnlyList<AircraftModel>> ListAsync(CancellationToken ct = default);

    // Retorna los modelos de un fabricante específico
    Task<IReadOnlyList<AircraftModel>> ListByManufacturerAsync(int idManufacturer, CancellationToken ct = default);

    // Agrega un nuevo modelo al sistema
    Task AddAsync(AircraftModel aircraftModel, CancellationToken ct = default);

    // Actualiza los datos de un modelo existente
    Task UpdateAsync(AircraftModel aircraftModel, CancellationToken ct = default);

    // Elimina un modelo del sistema por su ID
    Task DeleteAsync(AircraftModelId id, CancellationToken ct = default);
}
