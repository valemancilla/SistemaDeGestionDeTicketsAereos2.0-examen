// Caso de uso: obtener todos los tipos de equipaje disponibles en el sistema
using SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Application.UseCases;

public sealed class GetAllBaggageTypesUseCase
{
    private readonly IBaggageTypeRepository _repo;

    public GetAllBaggageTypesUseCase(IBaggageTypeRepository repo) => _repo = repo;

    // Delega directamente al repositorio sin lógica adicional
    public async Task<IReadOnlyList<BaggageType>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.ListAsync(ct);
}
