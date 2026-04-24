// Caso de uso: obtener todos los emails de clientes registrados en el sistema
using SistemaDeGestionDeTicketsAereos.src.modules.customerEmail.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.customerEmail.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.customerEmail.Application.UseCases;

public sealed class GetAllCustomerEmailsUseCase
{
    private readonly ICustomerEmailRepository _repo;
    public GetAllCustomerEmailsUseCase(ICustomerEmailRepository repo) => _repo = repo;

    // Delega directamente al repositorio sin filtros adicionales
    public async Task<IReadOnlyList<CustomerEmail>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.ListAsync(ct);
}
