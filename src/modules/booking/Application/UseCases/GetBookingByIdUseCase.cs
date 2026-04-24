// Caso de uso: buscar una reserva por su ID, lanza excepción si no se encuentra
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.booking.Application.UseCases;

public sealed class GetBookingByIdUseCase
{
    private readonly IBookingRepository _repo;

    public GetBookingByIdUseCase(IBookingRepository repo) => _repo = repo;

    // Lanza KeyNotFoundException si la reserva no existe, para que la UI pueda mostrar el mensaje apropiado
    public async Task<Booking> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(BookingId.Create(id), ct);
        if (entity is null) throw new KeyNotFoundException($"Booking with id '{id}' was not found.");
        return entity;
    }
}
