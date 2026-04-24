// Contrato del servicio de check-in: define las operaciones disponibles para la capa de presentación
// El alias evita el conflicto entre el namespace "CheckIn" y el tipo "CheckIn"
using CheckInAggregate = global::SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Domain.aggregate.CheckIn;

namespace SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Application.Interfaces;

// Interfaz del servicio de check-in — desacopla la capa UI del servicio concreto
public interface ICheckInService
{
    // Registra un nuevo check-in vinculando ticket, canal, asiento, usuario y estado
    Task<CheckInAggregate> CreateAsync(DateTime date, int idTicket, int idChannel, int idSeat, int idUser, int idStatus, CancellationToken cancellationToken = default);

    // Busca un check-in por su ID, retorna null si no existe
    Task<CheckInAggregate?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    // Retorna todos los check-ins registrados en el sistema
    Task<IReadOnlyCollection<CheckInAggregate>> GetAllAsync(CancellationToken cancellationToken = default);

    // Actualiza los datos de un check-in existente, lanza excepción si no se encuentra
    Task<CheckInAggregate> UpdateAsync(int id, DateTime date, int idTicket, int idChannel, int idSeat, int idUser, int idStatus, CancellationToken cancellationToken = default);

    // Elimina un check-in por su ID, retorna false si no existe
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
