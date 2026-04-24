// Caso de uso: buscar un modelo de aeronave por su ID, lanza excepción si no se encuentra
using SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Application.UseCases;

public sealed class GetAircraftModelByIdUseCase
{
    private readonly IAircraftModelRepository _repo;

    public GetAircraftModelByIdUseCase(IAircraftModelRepository repo) => _repo = repo;

    // Lanza KeyNotFoundException si el modelo no existe, para que la UI pueda mostrar el mensaje apropiado
    public async Task<AircraftModel> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(AircraftModelId.Create(id), ct);
        if (entity is null) throw new KeyNotFoundException($"AircraftModel with id '{id}' was not found.");
        return entity;
    }
}
