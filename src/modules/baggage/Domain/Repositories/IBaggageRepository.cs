// Contrato del repositorio de equipajes: define las operaciones de persistencia disponibles
using SistemaDeGestionDeTicketsAereos.src.modules.baggage.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.baggage.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.baggage.Domain.Repositories;

// Interfaz que define las operaciones CRUD del repositorio de equipajes
public interface IBaggageRepository
{
    // Busca un equipaje por su ID
    Task<Baggage?> GetByIdAsync(BaggageId id, CancellationToken ct = default);

    // Retorna todos los equipajes del sistema
    Task<IReadOnlyList<Baggage>> ListAsync(CancellationToken ct = default);

    // Retorna todos los equipajes asociados a un ticket específico
    Task<IReadOnlyList<Baggage>> ListByTicketAsync(int idTicket, CancellationToken ct = default);

    // Agrega un nuevo equipaje al sistema
    Task AddAsync(Baggage baggage, CancellationToken ct = default);

    // Actualiza los datos de un equipaje existente
    Task UpdateAsync(Baggage baggage, CancellationToken ct = default);

    // Elimina un equipaje del sistema por su ID
    Task DeleteAsync(BaggageId id, CancellationToken ct = default);
}
