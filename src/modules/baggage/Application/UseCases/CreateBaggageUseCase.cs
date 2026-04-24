// Caso de uso: registrar un nuevo equipaje asociado a un tiquete y tipo de equipaje
using SistemaDeGestionDeTicketsAereos.src.modules.baggage.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.baggage.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.baggage.Application.UseCases;

public sealed class CreateBaggageUseCase
{
    private readonly IBaggageRepository _repo;

    public CreateBaggageUseCase(IBaggageRepository repo) => _repo = repo;

    // Las validaciones (peso > 0, IDs válidos) las maneja el agregado Baggage.CreateNew
    public async Task<Baggage> ExecuteAsync(decimal weight, int idTicket, int idBaggageType, CancellationToken ct = default)
    {
        var entity = Baggage.CreateNew(weight, idTicket, idBaggageType);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}
