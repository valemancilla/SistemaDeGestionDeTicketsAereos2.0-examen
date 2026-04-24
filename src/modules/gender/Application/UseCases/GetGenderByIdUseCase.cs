using SistemaDeGestionDeTicketsAereos.src.modules.gender.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.gender.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.gender.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.gender.Application.UseCases;

public sealed class GetGenderByIdUseCase
{
    private readonly IGenderRepository _repo;
    public GetGenderByIdUseCase(IGenderRepository repo) => _repo = repo;

    public async Task<Gender> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(GenderId.Create(id), ct);
        if (entity is null) throw new KeyNotFoundException($"Gender with id '{id}' was not found.");
        return entity;
    }
}
