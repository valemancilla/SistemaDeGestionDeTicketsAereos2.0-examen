// Caso de uso: obtener todos los países registrados en el sistema
using SistemaDeGestionDeTicketsAereos.src.modules.country.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.country.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.country.Application.UseCases;

public sealed class GetAllCountriesUseCase
{
    private readonly ICountryRepository _repo;
    public GetAllCountriesUseCase(ICountryRepository repo) => _repo = repo;

    // Delega directamente al repositorio sin filtros adicionales
    public async Task<IReadOnlyList<Country>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.ListAsync(ct);
}
