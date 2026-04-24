using SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Application.UseCases;

public sealed class GetManufacturerByIdUseCase
{
    private readonly IManufacturerRepository _repo;
    public GetManufacturerByIdUseCase(IManufacturerRepository repo) => _repo = repo;

    public async Task<Manufacturer> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(ManufacturerId.Create(id), ct);
        if (entity is null) throw new KeyNotFoundException($"Manufacturer with id '{id}' was not found.");
        return entity;
    }
}
