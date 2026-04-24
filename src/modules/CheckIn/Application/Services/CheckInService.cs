// Implementación del servicio de check-in: coordina el repositorio y la unidad de trabajo
// El alias evita el conflicto entre el namespace "CheckIn" y el tipo "CheckIn"
using CheckInAggregate = global::SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Domain.aggregate.CheckIn;
using SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Application.Services;

// Servicio de aplicación de check-in — orquesta sin lógica de dominio propia
public sealed class CheckInService : ICheckInService
{
    private readonly ICheckInRepository _checkInRepository;
    private readonly IUnitOfWork _unitOfWork;

    // Inyección de dependencias: el repositorio y la unidad de trabajo llegan por constructor
    public CheckInService(ICheckInRepository checkInRepository, IUnitOfWork unitOfWork)
    {
        _checkInRepository = checkInRepository;
        _unitOfWork = unitOfWork;
    }

    // Registra el check-in y lo persiste inmediatamente
    public async Task<CheckInAggregate> CreateAsync(DateTime date, int idTicket, int idChannel, int idSeat, int idUser, int idStatus, CancellationToken cancellationToken = default)
    {
        var entity = CheckInAggregate.CreateNew(date, idTicket, idChannel, idSeat, idUser, idStatus);
        await _checkInRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    // Busca un check-in por ID delegando directamente al repositorio
    public Task<CheckInAggregate?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _checkInRepository.GetByIdAsync(CheckInId.Create(id), cancellationToken);
    }

    // Retorna todos los check-ins sin filtro
    public async Task<IReadOnlyCollection<CheckInAggregate>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _checkInRepository.ListAsync(cancellationToken);
    }

    // Actualiza un check-in verificando que exista, luego recrea el agregado con los nuevos datos
    public async Task<CheckInAggregate> UpdateAsync(int id, DateTime date, int idTicket, int idChannel, int idSeat, int idUser, int idStatus, CancellationToken cancellationToken = default)
    {
        var checkInId = CheckInId.Create(id);
        var existing = await _checkInRepository.GetByIdAsync(checkInId, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"CheckIn with id '{id}' was not found.");

        var updated = CheckInAggregate.Create(id, date, idTicket, idChannel, idSeat, idUser, idStatus);
        await _checkInRepository.UpdateAsync(updated, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return updated;
    }

    // Elimina un check-in por su ID, retorna false si no existe en lugar de lanzar excepción
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var checkInId = CheckInId.Create(id);
        var existing = await _checkInRepository.GetByIdAsync(checkInId, cancellationToken);
        if (existing is null)
            return false;

        await _checkInRepository.DeleteAsync(checkInId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
