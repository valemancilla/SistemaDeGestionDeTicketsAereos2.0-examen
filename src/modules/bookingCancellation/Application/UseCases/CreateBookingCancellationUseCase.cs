// Caso de uso: registrar una nueva cancelación de reserva con su motivo y penalización
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Application.UseCases;

public sealed class CreateBookingCancellationUseCase
{
    private readonly IBookingCancellationRepository _repo;

    public CreateBookingCancellationUseCase(IBookingCancellationRepository repo) => _repo = repo;

    // Las validaciones (fecha no futura, motivo obligatorio, penalización >= 0) las hace el agregado
    public async Task<BookingCancellation> ExecuteAsync(DateTime cancellationDate, string reason, decimal penaltyAmount, int idBooking, int idUser, CancellationToken ct = default)
    {
        var entity = BookingCancellation.CreateNew(cancellationDate, reason, penaltyAmount, idBooking, idUser);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}
