// Caso de uso: eliminar una cancelación de reserva por su ID
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Application.UseCases;

public sealed class DeleteBookingCancellationUseCase
{
    private readonly IBookingCancellationRepository _repo;
    public DeleteBookingCancellationUseCase(IBookingCancellationRepository repo) => _repo = repo;

    // Retorna false en lugar de lanzar excepción — permite que la UI decida cómo manejar el caso
    public async Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(BookingCancellationId.Create(id), ct);
        if (existing is null) return false;
        await _repo.DeleteAsync(BookingCancellationId.Create(id), ct);
        return true;
    }
}
