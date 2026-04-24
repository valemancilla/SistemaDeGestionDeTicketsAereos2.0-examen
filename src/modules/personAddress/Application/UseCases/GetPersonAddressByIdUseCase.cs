using SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Application.UseCases;

public sealed class GetPersonAddressByIdUseCase
{
    private readonly IPersonAddressRepository _repo;
    public GetPersonAddressByIdUseCase(IPersonAddressRepository repo) => _repo = repo;

    public async Task<PersonAddress> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(PersonAddressId.Create(id), ct);
        if (entity is null) throw new KeyNotFoundException($"PersonAddress with id '{id}' was not found.");
        return entity;
    }
}
