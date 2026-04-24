using SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Application.UseCases;

public sealed class CreatePersonAddressUseCase
{
    private readonly IPersonAddressRepository _repo;
    public CreatePersonAddressUseCase(IPersonAddressRepository repo) => _repo = repo;

    public async Task<PersonAddress> ExecuteAsync(string street, string number, string neighborhood, string dwellingType, string? zipCode, int idPerson, int idCity, bool active, CancellationToken ct = default)
    {
        var entity = PersonAddress.CreateNew(street, number, neighborhood, dwellingType, zipCode, idPerson, idCity, active);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}
