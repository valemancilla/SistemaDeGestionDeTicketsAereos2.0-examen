// Caso de uso: buscar un equipaje por su ID, lanza excepción si no se encuentra
using SistemaDeGestionDeTicketsAereos.src.modules.baggage.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.baggage.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.baggage.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.baggage.Application.UseCases;

public sealed class GetBaggageByIdUseCase
{
    private readonly IBaggageRepository _repo;

    public GetBaggageByIdUseCase(IBaggageRepository repo) => _repo = repo;

    // Lanza KeyNotFoundException si el equipaje no existe, para que la UI pueda mostrar el mensaje apropiado
    public async Task<Baggage> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(BaggageId.Create(id), ct);
        if (entity is null) throw new KeyNotFoundException($"Baggage with id '{id}' was not found.");
        return entity;
    }
}
