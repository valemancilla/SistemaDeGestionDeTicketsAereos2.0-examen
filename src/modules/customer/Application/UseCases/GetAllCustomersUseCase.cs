// Caso de uso: obtener todos los clientes registrados en el sistema
using SistemaDeGestionDeTicketsAereos.src.modules.customer.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.customer.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.customer.Application.UseCases;

public sealed class GetAllCustomersUseCase
{
    private readonly ICustomerRepository _repo;
    public GetAllCustomersUseCase(ICustomerRepository repo) => _repo = repo;

    // Delega directamente al repositorio sin filtros adicionales
    public async Task<IReadOnlyList<Customer>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.ListAsync(ct);
}
