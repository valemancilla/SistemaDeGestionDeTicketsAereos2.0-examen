// Caso de uso: obtener todas las transiciones de estado de reserva registradas en el sistema
using SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Application.UseCases;

public sealed class GetAllBookingStatusHistoriesUseCase
{
    private readonly IBookingStatusHistoryRepository _repo;
    public GetAllBookingStatusHistoriesUseCase(IBookingStatusHistoryRepository repo) => _repo = repo;

    // Delega directamente al repositorio sin filtros adicionales
    public async Task<IReadOnlyList<BookingStatusHistory>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.ListAsync(ct);
}
