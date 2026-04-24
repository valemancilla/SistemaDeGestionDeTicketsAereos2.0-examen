// Implementación del servicio de clientes: coordina el repositorio y la unidad de trabajo
using SistemaDeGestionDeTicketsAereos.src.modules.customer.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.customer.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.customer.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.customer.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.customer.Application.Services;

// Servicio de aplicación de clientes — orquesta sin lógica de dominio propia
public sealed class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    // Inyección de dependencias: el repositorio y la unidad de trabajo llegan por constructor
    public CustomerService(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    // Una persona solo puede registrarse como cliente una vez — se verifica por idPerson antes de persistir
    public async Task<Customer> CreateAsync(DateOnly registrationDate, int idPerson, bool active, CancellationToken cancellationToken = default)
    {
        var existing = await _customerRepository.GetByPersonIdAsync(idPerson, cancellationToken);
        if (existing is not null)
            throw new InvalidOperationException($"Customer for person '{idPerson}' already exists.");

        var entity = Customer.CreateNew(registrationDate, idPerson, active);
        await _customerRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    // Busca un cliente por ID delegando directamente al repositorio
    public Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _customerRepository.GetByIdAsync(CustomerId.Create(id), cancellationToken);
    }

    // Retorna todos los clientes sin filtro
    public async Task<IReadOnlyCollection<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _customerRepository.ListAsync(cancellationToken);
    }

    // Actualiza un cliente verificando que exista, luego recrea el agregado con los nuevos datos
    public async Task<Customer> UpdateAsync(int id, DateOnly registrationDate, int idPerson, bool active, CancellationToken cancellationToken = default)
    {
        var customerId = CustomerId.Create(id);
        var existing = await _customerRepository.GetByIdAsync(customerId, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"Customer with id '{id}' was not found.");

        var updated = Customer.Create(id, registrationDate, idPerson, active);
        await _customerRepository.UpdateAsync(updated, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return updated;
    }

    // Elimina un cliente por su ID, retorna false si no existe en lugar de lanzar excepción
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var customerId = CustomerId.Create(id);
        var existing = await _customerRepository.GetByIdAsync(customerId, cancellationToken);
        if (existing is null)
            return false;

        await _customerRepository.DeleteAsync(customerId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
