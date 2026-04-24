// Contrato del repositorio de tipos de equipaje: define las operaciones de persistencia disponibles
using SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Domain.Repositories;

// Interfaz que define las operaciones CRUD del repositorio de tipos de equipaje
public interface IBaggageTypeRepository
{
    // Busca un tipo de equipaje por su ID
    Task<BaggageType?> GetByIdAsync(BaggageTypeId id, CancellationToken ct = default);

    // Retorna todos los tipos de equipaje disponibles en el sistema
    Task<IReadOnlyList<BaggageType>> ListAsync(CancellationToken ct = default);

    // Agrega un nuevo tipo de equipaje al sistema
    Task AddAsync(BaggageType baggageType, CancellationToken ct = default);

    // Actualiza los datos de un tipo de equipaje existente
    Task UpdateAsync(BaggageType baggageType, CancellationToken ct = default);

    // Elimina un tipo de equipaje del sistema por su ID
    Task DeleteAsync(BaggageTypeId id, CancellationToken ct = default);
}
