// Caso de uso: obtener todos los canales de check-in registrados en el sistema
using SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Application.UseCases;

public sealed class GetAllCheckInChannelsUseCase
{
    private readonly ICheckInChannelRepository _repo;
    public GetAllCheckInChannelsUseCase(ICheckInChannelRepository repo) => _repo = repo;

    // Delega directamente al repositorio sin filtros adicionales
    public async Task<IReadOnlyList<CheckInChannel>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.ListAsync(ct);
}
