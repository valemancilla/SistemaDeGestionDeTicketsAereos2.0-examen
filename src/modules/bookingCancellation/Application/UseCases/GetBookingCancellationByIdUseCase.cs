// Caso de uso: buscar una cancelación de reserva por su ID, lanza excepción si no existe
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Application.UseCases;

public sealed class GetBookingCancellationByIdUseCase
{
    private readonly IBookingCancellationRepository _repo;
    public GetBookingCancellationByIdUseCase(IBookingCancellationRepository repo) => _repo = repo;

    // La excepción permite que el controlador o la UI devuelvan un 404 claro al usuario
    public async Task<BookingCancellation> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(BookingCancellationId.Create(id), ct);
        if (entity is null) throw new KeyNotFoundException($"BookingCancellation with id '{id}' was not found.");
        return entity;
    }
}
