// Implementación del servicio de emails de cliente: coordina el repositorio y la unidad de trabajo
using SistemaDeGestionDeTicketsAereos.src.modules.customerEmail.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.customerEmail.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.customerEmail.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.customerEmail.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.customerEmail.Application.Services;

// Servicio de aplicación de emails — orquesta sin lógica de dominio propia
public sealed class CustomerEmailService : ICustomerEmailService
{
    private readonly ICustomerEmailRepository _customerEmailRepository;
    private readonly IUnitOfWork _unitOfWork;

    // Inyección de dependencias: el repositorio y la unidad de trabajo llegan por constructor
    public CustomerEmailService(ICustomerEmailRepository customerEmailRepository, IUnitOfWork unitOfWork)
    {
        _customerEmailRepository = customerEmailRepository;
        _unitOfWork = unitOfWork;
    }

    // Registra el email y lo persiste inmediatamente
    public async Task<CustomerEmail> CreateAsync(string email, int idPerson, CancellationToken cancellationToken = default)
    {
        if (await _customerEmailRepository.IsEmailInUseAsync(email, null, cancellationToken))
            throw new InvalidOperationException("Ese correo electrónico ya está registrado; no se puede duplicar en el sistema.");
        var entity = CustomerEmail.CreateNew(email, idPerson);
        await _customerEmailRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    // Busca un email por ID delegando directamente al repositorio
    public Task<CustomerEmail?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _customerEmailRepository.GetByIdAsync(CustomerEmailId.Create(id), cancellationToken);
    }

    // Retorna todos los emails sin filtro
    public async Task<IReadOnlyCollection<CustomerEmail>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _customerEmailRepository.ListAsync(cancellationToken);
    }

    // Actualiza un email verificando que exista, luego recrea el agregado con los nuevos datos
    public async Task<CustomerEmail> UpdateAsync(int id, string email, int idPerson, CancellationToken cancellationToken = default)
    {
        var customerEmailId = CustomerEmailId.Create(id);
        var existing = await _customerEmailRepository.GetByIdAsync(customerEmailId, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"CustomerEmail with id '{id}' was not found.");

        if (await _customerEmailRepository.IsEmailInUseAsync(email, id, cancellationToken))
            throw new InvalidOperationException("Ese correo electrónico ya está registrado; no se puede duplicar en el sistema.");
        var updated = CustomerEmail.Create(id, email, idPerson);
        await _customerEmailRepository.UpdateAsync(updated, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return updated;
    }

    // Elimina un email por su ID, retorna false si no existe en lugar de lanzar excepción
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var customerEmailId = CustomerEmailId.Create(id);
        var existing = await _customerEmailRepository.GetByIdAsync(customerEmailId, cancellationToken);
        if (existing is null)
            return false;

        await _customerEmailRepository.DeleteAsync(customerEmailId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
