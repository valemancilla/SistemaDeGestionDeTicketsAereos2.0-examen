// Caso de uso: registrar una nueva ciudad asociada a un país
using SistemaDeGestionDeTicketsAereos.src.modules.city.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.city.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.city.Application.UseCases;

public sealed class CreateCityUseCase
{
    private readonly ICityRepository _repo;
    public CreateCityUseCase(ICityRepository repo) => _repo = repo;

    // La validación del nombre y la FK del país las hace el agregado
    public async Task<City> ExecuteAsync(string name, int idCountry, CancellationToken ct = default)
    {
        var entity = City.CreateNew(name, idCountry);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}
