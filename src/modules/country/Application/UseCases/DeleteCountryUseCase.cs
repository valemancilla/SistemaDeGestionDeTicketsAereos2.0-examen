// Caso de uso: eliminar un país por su ID
using SistemaDeGestionDeTicketsAereos.src.modules.country.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.country.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.country.Application.UseCases;

public sealed class DeleteCountryUseCase
{
    private readonly ICountryRepository _repo;
    public DeleteCountryUseCase(ICountryRepository repo) => _repo = repo;

    // Retorna false si no existe — evita lanzar excepción por un recurso que ya no está
    public async Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(CountryId.Create(id), ct);
        if (existing is null) return false;
        await _repo.DeleteAsync(CountryId.Create(id), ct);
        return true;
    }
}
