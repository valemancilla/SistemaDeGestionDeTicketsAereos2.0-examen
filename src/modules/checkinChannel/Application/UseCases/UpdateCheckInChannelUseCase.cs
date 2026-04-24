// Caso de uso: actualizar un canal de check-in existente verificando que exista antes de modificarlo
using SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Application.UseCases;

public sealed class UpdateCheckInChannelUseCase
{
    private readonly ICheckInChannelRepository _repo;
    public UpdateCheckInChannelUseCase(ICheckInChannelRepository repo) => _repo = repo;

    // Verifica que el canal exista antes de actualizarlo — recrea el agregado con los nuevos datos
    public async Task<CheckInChannel> ExecuteAsync(int id, string name, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(CheckInChannelId.Create(id), ct);
        if (existing is null) throw new KeyNotFoundException($"CheckInChannel with id '{id}' was not found.");
        var updated = CheckInChannel.Create(id, name);
        await _repo.UpdateAsync(updated, ct);
        return updated;
    }
}
