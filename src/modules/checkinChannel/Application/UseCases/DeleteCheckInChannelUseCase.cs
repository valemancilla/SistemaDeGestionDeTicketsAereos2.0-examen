// Caso de uso: eliminar un canal de check-in por su ID
using SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Application.UseCases;

public sealed class DeleteCheckInChannelUseCase
{
    private readonly ICheckInChannelRepository _repo;
    public DeleteCheckInChannelUseCase(ICheckInChannelRepository repo) => _repo = repo;

    // Retorna false si no existe — evita lanzar excepción por un recurso que ya no está
    public async Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(CheckInChannelId.Create(id), ct);
        if (existing is null) return false;
        await _repo.DeleteAsync(CheckInChannelId.Create(id), ct);
        return true;
    }
}
