using SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.manufacturer.Application.UseCases;

public sealed class GetAllManufacturersUseCase
{
    private readonly IManufacturerRepository _repo;
    public GetAllManufacturersUseCase(IManufacturerRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<Manufacturer>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.ListAsync(ct);
}
