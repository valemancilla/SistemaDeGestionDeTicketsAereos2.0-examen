// Caso de uso: buscar una aerolínea por su ID, lanza excepción si no se encuentra
using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.aeroline.Application.UseCases;

public sealed class GetAerolineByIdUseCase
{
    private readonly IAirlineRepository _repo;

    public GetAerolineByIdUseCase(IAirlineRepository repo) => _repo = repo;

    // Lanza KeyNotFoundException si la aerolínea no existe, para que la UI pueda mostrar el mensaje apropiado
    public async Task<Aeroline> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(AirlineId.Create(id), ct);
        if (entity is null) throw new KeyNotFoundException($"Aeroline with id '{id}' was not found.");
        return entity;
    }
}
