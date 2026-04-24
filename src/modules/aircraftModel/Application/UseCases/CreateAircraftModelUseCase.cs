// Caso de uso: registrar un nuevo modelo de aeronave asociado a su fabricante
using SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.aircraftModel.Application.UseCases;

public sealed class CreateAircraftModelUseCase
{
    private readonly IAircraftModelRepository _repo;

    public CreateAircraftModelUseCase(IAircraftModelRepository repo) => _repo = repo;

    // Las validaciones (nombre no vacío, fabricante > 0) las maneja el agregado AircraftModel.CreateNew
    public async Task<AircraftModel> ExecuteAsync(string name, int idManufacturer, CancellationToken ct = default)
    {
        var entity = AircraftModel.CreateNew(name, idManufacturer);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}
