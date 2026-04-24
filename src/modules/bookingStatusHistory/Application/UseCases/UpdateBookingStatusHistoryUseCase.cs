// Caso de uso: actualizar un registro del historial de estados verificando que exista antes de modificarlo
using SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Application.UseCases;

public sealed class UpdateBookingStatusHistoryUseCase
{
    private readonly IBookingStatusHistoryRepository _repo;
    public UpdateBookingStatusHistoryUseCase(IBookingStatusHistoryRepository repo) => _repo = repo;

    // Verifica que el registro exista antes de actualizarlo — recrea el agregado con los nuevos datos
    public async Task<BookingStatusHistory> ExecuteAsync(int id, DateTime changeDate, string? observation, int idBooking, int idStatus, int idUser, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(BookingStatusHistoryId.Create(id), ct);
        if (existing is null) throw new KeyNotFoundException($"BookingStatusHistory with id '{id}' was not found.");
        var updated = BookingStatusHistory.Create(id, changeDate, observation, idBooking, idStatus, idUser);
        await _repo.UpdateAsync(updated, ct);
        return updated;
    }
}
