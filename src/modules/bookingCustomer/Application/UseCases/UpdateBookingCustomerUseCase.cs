// Caso de uso: actualizar un pasajero de reserva existente verificando que exista antes de modificarlo
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Application.UseCases;

public sealed class UpdateBookingCustomerUseCase
{
    private readonly IBookingCustomerRepository _repo;
    public UpdateBookingCustomerUseCase(IBookingCustomerRepository repo) => _repo = repo;

    // Verifica que el pasajero exista antes de actualizarlo — recrea el agregado con los nuevos datos
    public async Task<BookingCustomer> ExecuteAsync(int id, DateTime associationDate, int idBooking, int idUser, int idPerson, int idSeat, bool isPrimary, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(BookingCustomerId.Create(id), ct);
        if (existing is null) throw new KeyNotFoundException($"BookingCustomer with id '{id}' was not found.");
        var updated = BookingCustomer.Create(id, associationDate, idBooking, idUser, idPerson, idSeat, isPrimary);
        await _repo.UpdateAsync(updated, ct);
        return updated;
    }
}
