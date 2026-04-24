// Caso de uso: obtener todas las reservas del sistema sin filtros
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.booking.Application.UseCases;

public sealed class GetAllBookingsUseCase
{
    private readonly IBookingRepository _repo;

    public GetAllBookingsUseCase(IBookingRepository repo) => _repo = repo;

    // Delega directamente al repositorio sin lógica adicional
    public async Task<IReadOnlyList<Booking>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.ListAsync(ct);
}
