// Caso de uso: buscar un canal de check-in por su ID, lanza excepción si no existe
using SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Application.UseCases;

public sealed class GetCheckInChannelByIdUseCase
{
    private readonly ICheckInChannelRepository _repo;
    public GetCheckInChannelByIdUseCase(ICheckInChannelRepository repo) => _repo = repo;

    // La excepción permite que el controlador o la UI devuelvan un 404 claro al usuario
    public async Task<CheckInChannel> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(CheckInChannelId.Create(id), ct);
        if (entity is null) throw new KeyNotFoundException($"CheckInChannel with id '{id}' was not found.");
        return entity;
    }
}
