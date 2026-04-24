// Caso de uso: registrar un nuevo check-in vinculando el ticket con el canal, asiento y estado
using SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Domain.Repositories;
using CheckInClass = global::SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Domain.aggregate.CheckIn;

namespace SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Application.UseCases;

public sealed class CreateCheckInUseCase
{
    private readonly ICheckInRepository _repo;
    public CreateCheckInUseCase(ICheckInRepository repo) => _repo = repo;

    // Las validaciones (fecha no futura, FKs válidas) las hace el agregado
    public async Task<CheckInClass> ExecuteAsync(DateTime date, int idTicket, int idChannel, int idSeat, int idUser, int idStatus, CancellationToken ct = default)
    {
        var entity = CheckInClass.CreateNew(date, idTicket, idChannel, idSeat, idUser, idStatus);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}
