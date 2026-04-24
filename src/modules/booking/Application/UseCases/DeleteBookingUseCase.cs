// Caso de uso: eliminar una reserva por su ID, retorna false si no existe en lugar de lanzar excepción
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.booking.Application.UseCases;

public sealed class DeleteBookingUseCase
{
    private readonly IBookingRepository _repo;

    public DeleteBookingUseCase(IBookingRepository repo) => _repo = repo;

    // Retorna false en lugar de lanzar excepción — permite que la UI decida cómo manejar el caso
    public async Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(BookingId.Create(id), ct);
        if (existing is null) return false;
        await _repo.DeleteAsync(BookingId.Create(id), ct);
        return true;
    }
}
