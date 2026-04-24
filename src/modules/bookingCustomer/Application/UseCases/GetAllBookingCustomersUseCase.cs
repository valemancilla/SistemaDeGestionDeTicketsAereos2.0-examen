// Caso de uso: obtener todos los pasajeros asociados a reservas registrados en el sistema
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Application.UseCases;

public sealed class GetAllBookingCustomersUseCase
{
    private readonly IBookingCustomerRepository _repo;
    public GetAllBookingCustomersUseCase(IBookingCustomerRepository repo) => _repo = repo;

    // Delega directamente al repositorio sin filtros adicionales
    public async Task<IReadOnlyList<BookingCustomer>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.ListAsync(ct);
}
