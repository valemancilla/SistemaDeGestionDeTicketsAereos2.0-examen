// Caso de uso: actualizar un equipaje existente verificando que exista antes de modificarlo
using SistemaDeGestionDeTicketsAereos.src.modules.baggage.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.baggage.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.baggage.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.baggage.Application.UseCases;

public sealed class UpdateBaggageUseCase
{
    private readonly IBaggageRepository _repo;

    public UpdateBaggageUseCase(IBaggageRepository repo) => _repo = repo;

    // Verifica que el equipaje exista antes de actualizarlo — recrea el agregado con los nuevos datos
    public async Task<Baggage> ExecuteAsync(int id, decimal weight, int idTicket, int idBaggageType, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(BaggageId.Create(id), ct);
        if (existing is null) throw new KeyNotFoundException($"Baggage with id '{id}' was not found.");
        var updated = Baggage.Create(id, weight, idTicket, idBaggageType);
        await _repo.UpdateAsync(updated, ct);
        return updated;
    }
}
