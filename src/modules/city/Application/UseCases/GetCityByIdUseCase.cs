// Caso de uso: buscar una ciudad por su ID, lanza excepción si no existe
using SistemaDeGestionDeTicketsAereos.src.modules.city.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.city.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.city.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.city.Application.UseCases;

public sealed class GetCityByIdUseCase
{
    private readonly ICityRepository _repo;
    public GetCityByIdUseCase(ICityRepository repo) => _repo = repo;

    // La excepción permite que el controlador o la UI devuelvan un 404 claro al usuario
    public async Task<City> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(CityId.Create(id), ct);
        if (entity is null) throw new KeyNotFoundException($"City with id '{id}' was not found.");
        return entity;
    }
}
