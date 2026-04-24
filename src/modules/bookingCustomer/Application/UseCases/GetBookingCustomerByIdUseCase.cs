// Caso de uso: buscar un pasajero de reserva por su ID, lanza excepción si no existe
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Application.UseCases;

public sealed class GetBookingCustomerByIdUseCase
{
    private readonly IBookingCustomerRepository _repo;
    public GetBookingCustomerByIdUseCase(IBookingCustomerRepository repo) => _repo = repo;

    // La excepción permite que el controlador o la UI devuelvan un 404 claro al usuario
    public async Task<BookingCustomer> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(BookingCustomerId.Create(id), ct);
        if (entity is null) throw new KeyNotFoundException($"BookingCustomer with id '{id}' was not found.");
        return entity;
    }
}
