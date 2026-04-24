// Caso de uso: obtener todas las cancelaciones de reserva registradas
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Application.UseCases;

public sealed class GetAllBookingCancellationsUseCase
{
    private readonly IBookingCancellationRepository _repo;
    public GetAllBookingCancellationsUseCase(IBookingCancellationRepository repo) => _repo = repo;

    // Delega directamente al repositorio sin filtros adicionales
    public async Task<IReadOnlyList<BookingCancellation>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.ListAsync(ct);
}
