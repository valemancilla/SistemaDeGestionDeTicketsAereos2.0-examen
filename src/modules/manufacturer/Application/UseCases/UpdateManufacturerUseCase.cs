using SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Application.UseCases;

public sealed class UpdateManufacturerUseCase
{
    private readonly IManufacturerRepository _repo;
    public UpdateManufacturerUseCase(IManufacturerRepository repo) => _repo = repo;

    public async Task<Manufacturer> ExecuteAsync(int id, string name, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(ManufacturerId.Create(id), ct);
        if (existing is null) throw new KeyNotFoundException($"Manufacturer with id '{id}' was not found.");
        var updated = Manufacturer.Create(id, name);
        await _repo.UpdateAsync(updated, ct);
        return updated;
    }
}
