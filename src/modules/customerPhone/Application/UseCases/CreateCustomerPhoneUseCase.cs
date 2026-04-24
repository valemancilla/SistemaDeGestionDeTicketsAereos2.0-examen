// Caso de uso: registrar un nuevo teléfono asociado a una persona del sistema
using SistemaDeGestionDeTicketsAereos.src.modules.customerPhone.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.customerPhone.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.customerPhone.Application.UseCases;

public sealed class CreateCustomerPhoneUseCase
{
    private readonly ICustomerPhoneRepository _repo;
    public CreateCustomerPhoneUseCase(ICustomerPhoneRepository repo) => _repo = repo;

    // La validación del formato del teléfono la hace el agregado
    public async Task<CustomerPhone> ExecuteAsync(string phone, int idPerson, CancellationToken ct = default)
    {
        if (await _repo.IsPhoneInUseAsync(phone, null, ct))
            throw new InvalidOperationException("Ese número de teléfono ya está registrado; no se puede duplicar en el sistema.");

        var entity = CustomerPhone.CreateNew(phone, idPerson);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}
