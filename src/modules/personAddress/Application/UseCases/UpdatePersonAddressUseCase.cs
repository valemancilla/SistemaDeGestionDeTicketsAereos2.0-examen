using SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.personAddress.Application.UseCases;

public sealed class UpdatePersonAddressUseCase
{
    private readonly IPersonAddressRepository _repo;
    public UpdatePersonAddressUseCase(IPersonAddressRepository repo) => _repo = repo;

    public async Task<PersonAddress> ExecuteAsync(int id, string street, string number, string neighborhood, string dwellingType, string? zipCode, int idPerson, int idCity, bool active, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(PersonAddressId.Create(id), ct);
        if (existing is null) throw new KeyNotFoundException($"PersonAddress with id '{id}' was not found.");
        var updated = PersonAddress.Create(id, street, number, neighborhood, dwellingType, zipCode, idPerson, idCity, active);
        await _repo.UpdateAsync(updated, ct);
        return updated;
    }
}
