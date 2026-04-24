// Contrato del repositorio de tiquetes: define las operaciones de persistencia disponibles
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.ticket.Domain.Repositories;

// Interfaz que define las operaciones CRUD del repositorio de tiquetes
public interface ITicketRepository
{
    // Busca un tiquete por su ID
    Task<Ticket?> GetByIdAsync(TicketId id, CancellationToken ct = default);

    // Busca un tiquete por su código único — útil para validación en abordaje
    Task<Ticket?> GetByCodeAsync(string ticketCode, CancellationToken ct = default);

    // Retorna todos los tiquetes del sistema
    Task<IReadOnlyList<Ticket>> ListAsync(CancellationToken ct = default);

    // Agrega un nuevo tiquete al sistema
    Task AddAsync(Ticket ticket, CancellationToken ct = default);

    // Actualiza los datos de un tiquete existente (ej: cambio de estado)
    Task UpdateAsync(Ticket ticket, CancellationToken ct = default);

    // Elimina un tiquete del sistema por su ID
    Task DeleteAsync(TicketId id, CancellationToken ct = default);
}
