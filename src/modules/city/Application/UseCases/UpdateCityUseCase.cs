// Caso de uso: actualizar una ciudad existente verificando que exista antes de modificarla
using SistemaDeGestionDeTicketsAereos.src.modules.city.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.city.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.city.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.city.Application.UseCases;

public sealed class UpdateCityUseCase
{
    private readonly ICityRepository _repo;
    public UpdateCityUseCase(ICityRepository repo) => _repo = repo;

    // Verifica que la ciudad exista antes de actualizarla — recrea el agregado con los nuevos datos
    public async Task<City> ExecuteAsync(int id, string name, int idCountry, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(CityId.Create(id), ct);
        if (existing is null) throw new KeyNotFoundException($"City with id '{id}' was not found.");
        var updated = City.Create(id, name, idCountry);
        await _repo.UpdateAsync(updated, ct);
        return updated;
    }
}
