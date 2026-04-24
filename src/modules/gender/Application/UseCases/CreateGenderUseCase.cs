using SistemaDeGestionDeTicketsAereos.src.modules.gender.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.gender.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.gender.Application.UseCases;

public sealed class CreateGenderUseCase
{
    private readonly IGenderRepository _repo;
    public CreateGenderUseCase(IGenderRepository repo) => _repo = repo;

    public async Task<Gender> ExecuteAsync(string description, CancellationToken ct = default)
    {
        var entity = Gender.CreateNew(description);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}
