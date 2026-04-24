// Caso de uso: obtener todos los equipajes registrados en el sistema
using SistemaDeGestionDeTicketsAereos.src.modules.baggage.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.baggage.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.baggage.Application.UseCases;

public sealed class GetAllBaggagesUseCase
{
    private readonly IBaggageRepository _repo;

    public GetAllBaggagesUseCase(IBaggageRepository repo) => _repo = repo;

    // Delega directamente al repositorio sin lógica adicional
    public async Task<IReadOnlyList<Baggage>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.ListAsync(ct);
}
