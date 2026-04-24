// Caso de uso: actualizar una cancelación de reserva existente verificando que exista antes de modificarla
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Application.UseCases;

public sealed class UpdateBookingCancellationUseCase
{
    private readonly IBookingCancellationRepository _repo;
    public UpdateBookingCancellationUseCase(IBookingCancellationRepository repo) => _repo = repo;

    // Verifica que la cancelación exista antes de actualizarla — recrea el agregado con los nuevos datos
    public async Task<BookingCancellation> ExecuteAsync(int id, DateTime cancellationDate, string reason, decimal penaltyAmount, int idBooking, int idUser, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(BookingCancellationId.Create(id), ct);
        if (existing is null) throw new KeyNotFoundException($"BookingCancellation with id '{id}' was not found.");
        var updated = BookingCancellation.Create(id, cancellationDate, reason, penaltyAmount, idBooking, idUser);
        await _repo.UpdateAsync(updated, ct);
        return updated;
    }
}
