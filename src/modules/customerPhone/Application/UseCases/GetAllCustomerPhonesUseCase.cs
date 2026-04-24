// Caso de uso: obtener todos los teléfonos de clientes registrados en el sistema
using SistemaDeGestionDeTicketsAereos.src.modules.customerPhone.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.customerPhone.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.customerPhone.Application.UseCases;

public sealed class GetAllCustomerPhonesUseCase
{
    private readonly ICustomerPhoneRepository _repo;
    public GetAllCustomerPhonesUseCase(ICustomerPhoneRepository repo) => _repo = repo;

    // Delega directamente al repositorio sin filtros adicionales
    public async Task<IReadOnlyList<CustomerPhone>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.ListAsync(ct);
}
