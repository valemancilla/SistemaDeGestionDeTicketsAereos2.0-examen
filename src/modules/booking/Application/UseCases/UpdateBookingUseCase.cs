// Caso de uso: actualizar una reserva existente verificando que exista antes de modificarla
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.booking.Application.UseCases;

public sealed class UpdateBookingUseCase
{
    private readonly IBookingRepository _repo;

    public UpdateBookingUseCase(IBookingRepository repo) => _repo = repo;

    // Verifica que la reserva exista antes de actualizarla — recrea el agregado con los nuevos datos
    public async Task<Booking> ExecuteAsync(int id, string code, DateTime flightDate, DateOnly creationDate, int seatCount, string? observations, int idFlight, int idStatus, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(BookingId.Create(id), ct);
        if (existing is null) throw new KeyNotFoundException($"Booking with id '{id}' was not found.");
        var updated = Booking.Create(id, code, flightDate, creationDate, seatCount, observations, idFlight, idStatus);
        await _repo.UpdateAsync(updated, ct);
        return updated;
    }
}
