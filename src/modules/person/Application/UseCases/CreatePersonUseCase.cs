using SistemaDeGestionDeTicketsAereos.src.modules.person.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.person.Application.UseCases;

public sealed class CreatePersonUseCase
{
    private readonly IPersonRepository _repo;
    public CreatePersonUseCase(IPersonRepository repo) => _repo = repo;

    public async Task<Person> ExecuteAsync(string firstName, string lastName, DateOnly birthDate, string documentNumber, int idDocumentType, int idGender, int idCountry, int? idAddress, CancellationToken ct = default)
    {
        var normalized = PersonDocumentNumber.Create(documentNumber).Value;
        var existing = await _repo.GetByDocumentNumberAsync(normalized, ct);
        if (existing is not null)
            throw new InvalidOperationException($"Ya existe una persona con el número de documento '{normalized}'.");

        var entity = Person.CreateNew(firstName, lastName, birthDate, documentNumber, idDocumentType, idGender, idCountry, idAddress);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}
