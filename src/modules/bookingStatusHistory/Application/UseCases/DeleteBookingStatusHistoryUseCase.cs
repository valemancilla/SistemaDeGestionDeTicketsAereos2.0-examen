// Caso de uso: eliminar un registro del historial de estados de reserva por su ID
using SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Application.UseCases;

public sealed class DeleteBookingStatusHistoryUseCase
{
    private readonly IBookingStatusHistoryRepository _repo;
    public DeleteBookingStatusHistoryUseCase(IBookingStatusHistoryRepository repo) => _repo = repo;

    // Retorna false si no existe — evita lanzar excepción por un registro que ya no está
    public async Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(BookingStatusHistoryId.Create(id), ct);
        if (existing is null) return false;
        await _repo.DeleteAsync(BookingStatusHistoryId.Create(id), ct);
        return true;
    }
}
