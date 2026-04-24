// Caso de uso: buscar un registro del historial de estados por su ID, lanza excepción si no existe
using SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Application.UseCases;

public sealed class GetBookingStatusHistoryByIdUseCase
{
    private readonly IBookingStatusHistoryRepository _repo;
    public GetBookingStatusHistoryByIdUseCase(IBookingStatusHistoryRepository repo) => _repo = repo;

    // La excepción permite que el controlador o la UI devuelvan un 404 claro al usuario
    public async Task<BookingStatusHistory> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(BookingStatusHistoryId.Create(id), ct);
        if (entity is null) throw new KeyNotFoundException($"BookingStatusHistory with id '{id}' was not found.");
        return entity;
    }
}
