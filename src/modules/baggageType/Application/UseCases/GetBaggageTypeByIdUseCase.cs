// Caso de uso: buscar un tipo de equipaje por su ID, lanza excepción si no se encuentra
using SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.baggageType.Application.UseCases;

public sealed class GetBaggageTypeByIdUseCase
{
    private readonly IBaggageTypeRepository _repo;

    public GetBaggageTypeByIdUseCase(IBaggageTypeRepository repo) => _repo = repo;

    // Lanza KeyNotFoundException si el tipo no existe, para que la UI pueda mostrar el mensaje apropiado
    public async Task<BaggageType> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(BaggageTypeId.Create(id), ct);
        if (entity is null) throw new KeyNotFoundException($"BaggageType with id '{id}' was not found.");
        return entity;
    }
}
