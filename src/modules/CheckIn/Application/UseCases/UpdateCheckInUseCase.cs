// Caso de uso: actualizar un check-in existente verificando que exista antes de modificarlo
using SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Domain.valueObject;
using CheckInClass = global::SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Domain.aggregate.CheckIn;

namespace SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Application.UseCases;

public sealed class UpdateCheckInUseCase
{
    private readonly ICheckInRepository _repo;
    public UpdateCheckInUseCase(ICheckInRepository repo) => _repo = repo;

    // Verifica que el check-in exista antes de actualizarlo — recrea el agregado con los nuevos datos
    public async Task<CheckInClass> ExecuteAsync(int id, DateTime date, int idTicket, int idChannel, int idSeat, int idUser, int idStatus, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(CheckInId.Create(id), ct);
        if (existing is null) throw new KeyNotFoundException($"CheckIn with id '{id}' was not found.");
        var updated = CheckInClass.Create(id, date, idTicket, idChannel, idSeat, idUser, idStatus);
        await _repo.UpdateAsync(updated, ct);
        return updated;
    }
}
