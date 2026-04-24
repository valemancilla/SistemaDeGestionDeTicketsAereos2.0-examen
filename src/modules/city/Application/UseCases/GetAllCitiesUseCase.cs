// Caso de uso: obtener todas las ciudades registradas en el sistema
using SistemaDeGestionDeTicketsAereos.src.modules.city.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.city.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.city.Application.UseCases;

public sealed class GetAllCitiesUseCase
{
    private readonly ICityRepository _repo;
    public GetAllCitiesUseCase(ICityRepository repo) => _repo = repo;

    // Delega directamente al repositorio sin filtros adicionales
    public async Task<IReadOnlyList<City>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.ListAsync(ct);
}
