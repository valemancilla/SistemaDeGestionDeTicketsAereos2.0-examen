// Caso de uso: buscar un teléfono de cliente por su ID, lanza excepción si no existe
using SistemaDeGestionDeTicketsAereos.src.modules.customerPhone.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.customerPhone.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.customerPhone.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.customerPhone.Application.UseCases;

public sealed class GetCustomerPhoneByIdUseCase
{
    private readonly ICustomerPhoneRepository _repo;
    public GetCustomerPhoneByIdUseCase(ICustomerPhoneRepository repo) => _repo = repo;

    // La excepción permite que el controlador o la UI devuelvan un 404 claro al usuario
    public async Task<CustomerPhone> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(CustomerPhoneId.Create(id), ct);
        if (entity is null) throw new KeyNotFoundException($"CustomerPhone with id '{id}' was not found.");
        return entity;
    }
}
