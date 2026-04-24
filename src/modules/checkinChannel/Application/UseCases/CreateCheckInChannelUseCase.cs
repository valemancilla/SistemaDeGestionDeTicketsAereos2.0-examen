// Caso de uso: registrar un nuevo canal de check-in
using SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Application.UseCases;

public sealed class CreateCheckInChannelUseCase
{
    private readonly ICheckInChannelRepository _repo;
    public CreateCheckInChannelUseCase(ICheckInChannelRepository repo) => _repo = repo;

    // La validación del nombre (no vacío, longitud) la hace el agregado
    public async Task<CheckInChannel> ExecuteAsync(string name, CancellationToken ct = default)
    {
        var entity = CheckInChannel.CreateNew(name);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}
