using SistemaDeGestionDeTicketsAereos.src.modules.person.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.person.Application.UseCases;

public sealed class UpdatePersonUseCase
{
    private readonly IPersonRepository _repo;
    public UpdatePersonUseCase(IPersonRepository repo) => _repo = repo;

    public async Task<Person> ExecuteAsync(int id, string firstName, string lastName, DateOnly birthDate, string documentNumber, int idDocumentType, int idGender, int idCountry, int? idAddress, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(PersonId.Create(id), ct);
        if (existing is null) throw new KeyNotFoundException($"Person with id '{id}' was not found.");
        var normalized = PersonDocumentNumber.Create(documentNumber).Value;
        var byNumber = await _repo.GetByDocumentNumberAsync(normalized, ct);
        if (byNumber is not null && byNumber.Id.Value != id)
            throw new InvalidOperationException($"Ya existe otra persona con el número de documento '{normalized}'.");

        var updated = Person.Create(id, firstName, lastName, birthDate, documentNumber, idDocumentType, idGender, idCountry, idAddress);
        await _repo.UpdateAsync(updated, ct);
        return updated;
    }
}
