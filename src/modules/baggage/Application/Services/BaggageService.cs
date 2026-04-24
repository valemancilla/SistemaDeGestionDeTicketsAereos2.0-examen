// Implementación del servicio de equipajes: coordina el repositorio y la unidad de trabajo
using SistemaDeGestionDeTicketsAereos.src.modules.baggage.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.baggage.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.baggage.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.baggage.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.baggage.Application.Services;

// Servicio de aplicación de equipajes — orquesta los casos de uso sin lógica de dominio propia
public sealed class BaggageService : IBaggageService
{
    private readonly IBaggageRepository _baggageRepository;
    private readonly IUnitOfWork _unitOfWork;

    // Inyección de dependencias: el repositorio y la unidad de trabajo llegan por constructor
    public BaggageService(IBaggageRepository baggageRepository, IUnitOfWork unitOfWork)
    {
        _baggageRepository = baggageRepository;
        _unitOfWork = unitOfWork;
    }

    // Crea un equipaje nuevo y lo persiste inmediatamente
    public async Task<Baggage> CreateAsync(decimal weight, int idTicket, int idBaggageType, CancellationToken cancellationToken = default)
    {
        var entity = Baggage.CreateNew(weight, idTicket, idBaggageType);
        await _baggageRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    // Busca un equipaje por ID delegando directamente al repositorio
    public Task<Baggage?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _baggageRepository.GetByIdAsync(BaggageId.Create(id), cancellationToken);
    }

    // Retorna todos los equipajes sin filtro
    public async Task<IReadOnlyCollection<Baggage>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _baggageRepository.ListAsync(cancellationToken);
    }

    // Actualiza un equipaje verificando que exista, luego recrea el agregado con los nuevos datos
    public async Task<Baggage> UpdateAsync(int id, decimal weight, int idTicket, int idBaggageType, CancellationToken cancellationToken = default)
    {
        var baggageId = BaggageId.Create(id);
        var existing = await _baggageRepository.GetByIdAsync(baggageId, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"Baggage with id '{id}' was not found.");

        var updated = Baggage.Create(id, weight, idTicket, idBaggageType);
        await _baggageRepository.UpdateAsync(updated, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return updated;
    }

    // Elimina un equipaje por su ID, retorna false si no existe en lugar de lanzar excepción
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var baggageId = BaggageId.Create(id);
        var existing = await _baggageRepository.GetByIdAsync(baggageId, cancellationToken);
        if (existing is null)
            return false;

        await _baggageRepository.DeleteAsync(baggageId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
