// Contrato del repositorio de tarifas: define las operaciones de persistencia disponibles
using SistemaDeGestionDeTicketsAereos.src.modules.fare.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.fare.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.fare.Domain.Repositories;

// Interfaz que define las operaciones CRUD del repositorio de tarifas
public interface IFareRepository
{
    // Busca una tarifa por su ID
    Task<Fare?> GetByIdAsync(FareId id, CancellationToken ct = default);

    // Retorna todas las tarifas registradas en el sistema
    Task<IReadOnlyList<Fare>> ListAsync(CancellationToken ct = default);

    // Agrega una nueva tarifa al sistema
    Task AddAsync(Fare fare, CancellationToken ct = default);

    // Actualiza los datos de una tarifa existente
    Task UpdateAsync(Fare fare, CancellationToken ct = default);

    // Elimina una tarifa del sistema por su ID
    Task DeleteAsync(FareId id, CancellationToken ct = default);
}
