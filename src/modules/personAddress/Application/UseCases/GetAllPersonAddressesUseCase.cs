using SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Application.UseCases;

public sealed class GetAllPersonAddressesUseCase
{
    private readonly IPersonAddressRepository _repo;
    public GetAllPersonAddressesUseCase(IPersonAddressRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<PersonAddress>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.ListAsync(ct);
}
