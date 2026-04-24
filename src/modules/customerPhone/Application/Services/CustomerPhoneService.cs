// Implementación del servicio de teléfonos de cliente: coordina el repositorio y la unidad de trabajo
using SistemaDeGestionDeTicketsAereos.src.modules.customerPhone.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.customerPhone.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.customerPhone.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.customerPhone.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.customerPhone.Application.Services;

// Servicio de aplicación de teléfonos — orquesta sin lógica de dominio propia
public sealed class CustomerPhoneService : ICustomerPhoneService
{
    private readonly ICustomerPhoneRepository _customerPhoneRepository;
    private readonly IUnitOfWork _unitOfWork;

    // Inyección de dependencias: el repositorio y la unidad de trabajo llegan por constructor
    public CustomerPhoneService(ICustomerPhoneRepository customerPhoneRepository, IUnitOfWork unitOfWork)
    {
        _customerPhoneRepository = customerPhoneRepository;
        _unitOfWork = unitOfWork;
    }

    // Registra el teléfono y lo persiste inmediatamente
    public async Task<CustomerPhone> CreateAsync(string phone, int idPerson, CancellationToken cancellationToken = default)
    {
        if (await _customerPhoneRepository.IsPhoneInUseAsync(phone, null, cancellationToken))
            throw new InvalidOperationException("Ese número de teléfono ya está registrado; no se puede duplicar en el sistema.");
        var entity = CustomerPhone.CreateNew(phone, idPerson);
        await _customerPhoneRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    // Busca un teléfono por ID delegando directamente al repositorio
    public Task<CustomerPhone?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _customerPhoneRepository.GetByIdAsync(CustomerPhoneId.Create(id), cancellationToken);
    }

    // Retorna todos los teléfonos sin filtro
    public async Task<IReadOnlyCollection<CustomerPhone>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _customerPhoneRepository.ListAsync(cancellationToken);
    }

    // Actualiza un teléfono verificando que exista, luego recrea el agregado con los nuevos datos
    public async Task<CustomerPhone> UpdateAsync(int id, string phone, int idPerson, CancellationToken cancellationToken = default)
    {
        var customerPhoneId = CustomerPhoneId.Create(id);
        var existing = await _customerPhoneRepository.GetByIdAsync(customerPhoneId, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"CustomerPhone with id '{id}' was not found.");

        if (await _customerPhoneRepository.IsPhoneInUseAsync(phone, id, cancellationToken))
            throw new InvalidOperationException("Ese número de teléfono ya está registrado; no se puede duplicar en el sistema.");
        var updated = CustomerPhone.Create(id, phone, idPerson);
        await _customerPhoneRepository.UpdateAsync(updated, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return updated;
    }

    // Elimina un teléfono por su ID, retorna false si no existe en lugar de lanzar excepción
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var customerPhoneId = CustomerPhoneId.Create(id);
        var existing = await _customerPhoneRepository.GetByIdAsync(customerPhoneId, cancellationToken);
        if (existing is null)
            return false;

        await _customerPhoneRepository.DeleteAsync(customerPhoneId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
