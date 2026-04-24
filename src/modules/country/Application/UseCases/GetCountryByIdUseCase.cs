// Caso de uso: buscar un país por su ID, lanza excepción si no existe
using SistemaDeGestionDeTicketsAereos.src.modules.country.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.country.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.country.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.country.Application.UseCases;

public sealed class GetCountryByIdUseCase
{
    private readonly ICountryRepository _repo;
    public GetCountryByIdUseCase(ICountryRepository repo) => _repo = repo;

    // La excepción permite que el controlador o la UI devuelvan un 404 claro al usuario
    public async Task<Country> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(CountryId.Create(id), ct);
        if (entity is null) throw new KeyNotFoundException($"Country with id '{id}' was not found.");
        return entity;
    }
}
