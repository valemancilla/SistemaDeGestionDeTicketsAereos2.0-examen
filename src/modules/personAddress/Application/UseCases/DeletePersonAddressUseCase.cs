using SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Application.UseCases;

public sealed class DeletePersonAddressUseCase
{
    private readonly IPersonAddressRepository _repo;
    public DeletePersonAddressUseCase(IPersonAddressRepository repo) => _repo = repo;

    public async Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(PersonAddressId.Create(id), ct);
        if (existing is null) return false;
        await _repo.DeleteAsync(PersonAddressId.Create(id), ct);
        return true;
    }
}
