// Caso de uso: eliminar un pasajero de reserva por su ID
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Application.UseCases;

public sealed class DeleteBookingCustomerUseCase
{
    private readonly IBookingCustomerRepository _repo;
    public DeleteBookingCustomerUseCase(IBookingCustomerRepository repo) => _repo = repo;

    // Retorna false si no existe — evita lanzar excepción por un recurso que ya no está
    public async Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(BookingCustomerId.Create(id), ct);
        if (existing is null) return false;
        await _repo.DeleteAsync(BookingCustomerId.Create(id), ct);
        return true;
    }
}
