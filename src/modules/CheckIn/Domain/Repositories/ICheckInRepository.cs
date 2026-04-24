// Contrato del repositorio de check-ins: define las operaciones de persistencia disponibles
// El alias CheckInClass es necesario porque el namespace "CheckIn" colisiona con el tipo "CheckIn"
using SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Domain.valueObject;
using CheckInClass = global::SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Domain.aggregate.CheckIn;

namespace SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Domain.Repositories;

// Interfaz que define las operaciones CRUD del repositorio de check-ins
public interface ICheckInRepository
{
    // Busca un check-in por su ID
    Task<CheckInClass?> GetByIdAsync(CheckInId id, CancellationToken ct = default);

    // Retorna todos los check-ins registrados en el sistema
    Task<IReadOnlyList<CheckInClass>> ListAsync(CancellationToken ct = default);

    // Agrega un nuevo check-in al sistema
    Task AddAsync(CheckInClass checkIn, CancellationToken ct = default);

    // Actualiza los datos de un check-in existente
    Task UpdateAsync(CheckInClass checkIn, CancellationToken ct = default);

    // Elimina un check-in del sistema por su ID
    Task DeleteAsync(CheckInId id, CancellationToken ct = default);
}
