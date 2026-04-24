// Caso de uso: eliminar una ciudad por su ID
using SistemaDeGestionDeTicketsAereos.src.modules.city.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.city.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.city.Application.UseCases;

public sealed class DeleteCityUseCase
{
    private readonly ICityRepository _repo;
    public DeleteCityUseCase(ICityRepository repo) => _repo = repo;

    // Retorna false si no existe — evita lanzar excepción por un recurso que ya no está
    public async Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(CityId.Create(id), ct);
        if (existing is null) return false;
        await _repo.DeleteAsync(CityId.Create(id), ct);
        return true;
    }
}
