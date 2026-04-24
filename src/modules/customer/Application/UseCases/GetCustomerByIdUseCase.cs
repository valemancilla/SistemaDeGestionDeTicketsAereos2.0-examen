// Caso de uso: buscar un cliente por su ID, lanza excepción si no existe
using SistemaDeGestionDeTicketsAereos.src.modules.customer.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.customer.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.customer.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.customer.Application.UseCases;

public sealed class GetCustomerByIdUseCase
{
    private readonly ICustomerRepository _repo;
    public GetCustomerByIdUseCase(ICustomerRepository repo) => _repo = repo;

    // La excepción permite que el controlador o la UI devuelvan un 404 claro al usuario
    public async Task<Customer> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(CustomerId.Create(id), ct);
        if (entity is null) throw new KeyNotFoundException($"Customer with id '{id}' was not found.");
        return entity;
    }
}
