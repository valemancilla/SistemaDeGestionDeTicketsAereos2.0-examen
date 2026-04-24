// Contrato del repositorio del historial de estados de tiquetes: define las operaciones de persistencia
using SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.ticketStatusHistory.Domain.Repositories;

// Interfaz que define las operaciones del repositorio de historial de estados de tiquete
public interface ITicketStatusHistoryRepository
{
    // Busca un registro del historial por su ID
    Task<TicketStatusHistory?> GetByIdAsync(TicketStatusHistoryId id, CancellationToken ct = default);

    // Retorna todos los registros del historial en el sistema
    Task<IReadOnlyList<TicketStatusHistory>> ListAsync(CancellationToken ct = default);

    // Retorna todos los cambios de estado de un tiquete específico — útil para auditoría
    Task<IReadOnlyList<TicketStatusHistory>> ListByTicketAsync(int idTicket, CancellationToken ct = default);

    // Agrega un nuevo registro al historial
    Task AddAsync(TicketStatusHistory history, CancellationToken ct = default);

    // Actualiza un registro existente del historial
    Task UpdateAsync(TicketStatusHistory history, CancellationToken ct = default);

    // Elimina un registro del historial por su ID
    Task DeleteAsync(TicketStatusHistoryId id, CancellationToken ct = default);
}
