// Contrato del repositorio de clases de asiento: define las operaciones de persistencia disponibles
using SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Domain.Repositories;

// Interfaz que define las operaciones CRUD del repositorio de clases de asiento
public interface ISeatClassRepository
{
    // Busca una clase de asiento por su ID
    Task<SeatClass?> GetByIdAsync(SeatClassId id, CancellationToken ct = default);

    // Retorna todas las clases de asiento disponibles en el sistema
    Task<IReadOnlyList<SeatClass>> ListAsync(CancellationToken ct = default);

    // Agrega una nueva clase de asiento al sistema
    Task AddAsync(SeatClass seatClass, CancellationToken ct = default);

    // Actualiza los datos de una clase de asiento existente
    Task UpdateAsync(SeatClass seatClass, CancellationToken ct = default);

    // Elimina una clase de asiento del sistema por su ID
    Task DeleteAsync(SeatClassId id, CancellationToken ct = default);
}
