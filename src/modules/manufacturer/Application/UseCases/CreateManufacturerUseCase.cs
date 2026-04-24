using SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Application.UseCases;

public sealed class CreateManufacturerUseCase
{
    private readonly IManufacturerRepository _repo;
    public CreateManufacturerUseCase(IManufacturerRepository repo) => _repo = repo;

    public async Task<Manufacturer> ExecuteAsync(string name, CancellationToken ct = default)
    {
        var existing = await _repo.GetByNameAsync(name, ct);
        if (existing is not null) throw new InvalidOperationException($"Manufacturer '{name}' already exists.");
        var entity = Manufacturer.CreateNew(name);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}
