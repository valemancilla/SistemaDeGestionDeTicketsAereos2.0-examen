// Caso de uso: registrar un nuevo tipo de equipaje en el sistema
using SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Application.UseCases;

public sealed class CreateBaggageTypeUseCase
{
    private readonly IBaggageTypeRepository _repo;

    public CreateBaggageTypeUseCase(IBaggageTypeRepository repo) => _repo = repo;

    // Las validaciones (nombre no vacío, longitud) las maneja el VO BaggageTypeName dentro del agregado
    public async Task<BaggageType> ExecuteAsync(
        string name,
        decimal weightKg,
        decimal basePriceCop,
        string? description,
        bool isActive,
        CancellationToken ct = default)
    {
        var entity = BaggageType.CreateNew(name, weightKg, basePriceCop, description, isActive);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}
