// Caso de uso: actualizar una aerolínea existente verificando que exista antes de modificarla
using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Application.UseCases;

public sealed class UpdateAerolineUseCase
{
    private readonly IAirlineRepository _repo;

    public UpdateAerolineUseCase(IAirlineRepository repo) => _repo = repo;

    // Verifica que la aerolínea exista antes de actualizarla — recrea el agregado con los nuevos datos
    public async Task<Aeroline> ExecuteAsync(int id, string name, string iataCode, int idCountry, bool active, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(AirlineId.Create(id), ct);
        if (existing is null) throw new KeyNotFoundException($"Aeroline with id '{id}' was not found.");

        var existingByName = await _repo.GetByNameAsync(name, ct);
        if (existingByName is not null && existingByName.Id.Value != id)
            throw new InvalidOperationException($"Aeroline with name '{name}' already exists.");

        var existingByIata = await _repo.GetByIataCodeAsync(iataCode, ct);
        if (existingByIata is not null && existingByIata.Id.Value != id)
            throw new InvalidOperationException($"Aeroline with IATA code '{iataCode}' already exists.");

        var updated = Aeroline.Create(id, name, iataCode, idCountry, active);
        await _repo.UpdateAsync(updated, ct);
        return updated;
    }
}
