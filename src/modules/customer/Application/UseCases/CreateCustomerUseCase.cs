// Caso de uso: registrar un nuevo cliente verificando que la persona no esté registrada antes
using SistemaDeGestionDeTicketsAereos.src.modules.customer.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.customer.Domain.Repositories;

namespace SistemaDeGestionDeTicketsAereos.src.modules.customer.Application.UseCases;

public sealed class CreateCustomerUseCase
{
    private readonly ICustomerRepository _repo;
    public CreateCustomerUseCase(ICustomerRepository repo) => _repo = repo;

    // Una persona no puede ser cliente dos veces — se busca por idPerson antes de crear
    public async Task<Customer> ExecuteAsync(DateOnly registrationDate, int idPerson, bool active, CancellationToken ct = default)
    {
        if (registrationDate <= DateOnly.FromDateTime(DateTime.Today))
            throw new InvalidOperationException("La fecha de registro del cliente debe ser posterior a la fecha de hoy.");

        var existing = await _repo.GetByPersonIdAsync(idPerson, ct);
        if (existing is not null) throw new InvalidOperationException($"Customer for person '{idPerson}' already exists.");
        var entity = Customer.CreateNew(registrationDate, idPerson, active);
        await _repo.AddAsync(entity, ct);
        return entity;
    }
}
