// Caso de uso: actualizar un país existente verificando que exista antes de modificarlo
using SistemaDeGestionDeTicketsAereos.src.modules.country.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.country.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.country.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.country.Application.UseCases;

public sealed class UpdateCountryUseCase
{
    private readonly ICountryRepository _repo;
    public UpdateCountryUseCase(ICountryRepository repo) => _repo = repo;

    // Verifica que el país exista antes de actualizarlo — recrea el agregado con los nuevos datos
    public async Task<Country> ExecuteAsync(int id, string name, string isoCode, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(CountryId.Create(id), ct);
        if (existing is null) throw new KeyNotFoundException($"Country with id '{id}' was not found.");
        var updated = Country.Create(id, name, isoCode);
        await _repo.UpdateAsync(updated, ct);
        return updated;
    }
}
