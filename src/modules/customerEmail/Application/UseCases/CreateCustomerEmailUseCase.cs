// Caso de uso: registrar un nuevo email asociado a una persona del sistema
using SistemaDeGestionDeTicketsAereos.src.modules.customerEmail.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.customerEmail.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.customerEmail.Application.UseCases;

public sealed class CreateCustomerEmailUseCase
{
    private readonly ICustomerEmailRepository _repo;
    public CreateCustomerEmailUseCase(ICustomerEmailRepository repo) => _repo = repo;

    // La validación del formato del email la hace el agregado
    public async Task<CustomerEmail> ExecuteAsync(string email, int idPerson, CancellationToken ct = default)
    {
        if (await _repo.IsEmailInUseAsync(email, null, ct))
            throw new InvalidOperationException("Ese correo electrónico ya está registrado; no se puede duplicar en el sistema.");

        var entity = CustomerEmail.CreateNew(email, idPerson);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}
