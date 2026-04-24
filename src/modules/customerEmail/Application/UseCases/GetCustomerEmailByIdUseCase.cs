// Caso de uso: buscar un email de cliente por su ID, lanza excepción si no existe
using SistemaDeGestionDeTicketsAereos.src.modules.customerEmail.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.customerEmail.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.customerEmail.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.customerEmail.Application.UseCases;

public sealed class GetCustomerEmailByIdUseCase
{
    private readonly ICustomerEmailRepository _repo;
    public GetCustomerEmailByIdUseCase(ICustomerEmailRepository repo) => _repo = repo;

    // La excepción permite que el controlador o la UI devuelvan un 404 claro al usuario
    public async Task<CustomerEmail> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(CustomerEmailId.Create(id), ct);
        if (entity is null) throw new KeyNotFoundException($"CustomerEmail with id '{id}' was not found.");
        return entity;
    }
}
