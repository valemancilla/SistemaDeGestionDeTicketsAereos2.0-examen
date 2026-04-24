using SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Application.UseCases;

public sealed class DeleteManufacturerUseCase
{
    private readonly IManufacturerRepository _repo;
    public DeleteManufacturerUseCase(IManufacturerRepository repo) => _repo = repo;

    public async Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(ManufacturerId.Create(id), ct);
        if (existing is null) return false;
        await _repo.DeleteAsync(ManufacturerId.Create(id), ct);
        return true;
    }
}
