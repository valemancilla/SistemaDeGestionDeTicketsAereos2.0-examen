// Caso de uso: registrar una nueva transición de estado en el historial de una reserva
using SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Application.UseCases;

public sealed class CreateBookingStatusHistoryUseCase
{
    private readonly IBookingStatusHistoryRepository _repo;
    public CreateBookingStatusHistoryUseCase(IBookingStatusHistoryRepository repo) => _repo = repo;

    // Las validaciones (fecha no futura, observación opcional hasta 500 chars) las hace el agregado
    public async Task<BookingStatusHistory> ExecuteAsync(DateTime changeDate, string? observation, int idBooking, int idStatus, int idUser, CancellationToken ct = default)
    {
        var entity = BookingStatusHistory.CreateNew(changeDate, observation, idBooking, idStatus, idUser);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}
