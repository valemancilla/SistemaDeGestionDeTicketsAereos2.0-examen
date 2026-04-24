// Caso de uso: asociar un nuevo pasajero a una reserva indicando si es el titular
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Application.UseCases;

public sealed class CreateBookingCustomerUseCase
{
    private readonly IBookingCustomerRepository _repo;
    public CreateBookingCustomerUseCase(IBookingCustomerRepository repo) => _repo = repo;

    // Las validaciones (fecha no futura, FKs válidas) las hace el agregado
    public async Task<BookingCustomer> ExecuteAsync(DateTime associationDate, int idBooking, int idUser, int idPerson, int idSeat, bool isPrimary, CancellationToken ct = default)
    {
        var entity = BookingCustomer.CreateNew(associationDate, idBooking, idUser, idPerson, idSeat, isPrimary);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}
