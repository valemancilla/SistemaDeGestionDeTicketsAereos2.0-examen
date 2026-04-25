// Caso de uso: actualizar un tipo de equipaje existente verificando que exista antes de modificarlo
using SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Application.UseCases;

public sealed class UpdateBaggageTypeUseCase
{
    private readonly IBaggageTypeRepository _repo;

    public UpdateBaggageTypeUseCase(IBaggageTypeRepository repo) => _repo = repo;

    // Verifica que el tipo exista antes de actualizarlo — recrea el agregado con los nuevos datos
    public async Task<BaggageType> ExecuteAsync(
        int id,
        string name,
        decimal weightKg,
        decimal basePriceCop,
        string? description,
        bool isActive,
        CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(BaggageTypeId.Create(id), ct);
        if (existing is null) throw new KeyNotFoundException($"BaggageType with id '{id}' was not found.");
        var updated = BaggageType.Create(id, name, weightKg, basePriceCop, description, isActive);
        await _repo.UpdateAsync(updated, ct);
        return updated;
    }
}
