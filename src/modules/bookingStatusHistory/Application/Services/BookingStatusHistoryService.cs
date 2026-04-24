// Implementación del servicio de historial de estados de reserva: coordina el repositorio y la unidad de trabajo
using SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Application.Services;

// Servicio de aplicación de historial — orquesta sin lógica de dominio propia
public sealed class BookingStatusHistoryService : IBookingStatusHistoryService
{
    private readonly IBookingStatusHistoryRepository _bookingStatusHistoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    // Inyección de dependencias: el repositorio y la unidad de trabajo llegan por constructor
    public BookingStatusHistoryService(IBookingStatusHistoryRepository bookingStatusHistoryRepository, IUnitOfWork unitOfWork)
    {
        _bookingStatusHistoryRepository = bookingStatusHistoryRepository;
        _unitOfWork = unitOfWork;
    }

    // Registra la transición de estado y la persiste inmediatamente
    public async Task<BookingStatusHistory> CreateAsync(DateTime changeDate, string? observation, int idBooking, int idStatus, int idUser, CancellationToken cancellationToken = default)
    {
        var entity = BookingStatusHistory.CreateNew(changeDate, observation, idBooking, idStatus, idUser);
        await _bookingStatusHistoryRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    // Busca un registro por ID delegando directamente al repositorio
    public Task<BookingStatusHistory?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _bookingStatusHistoryRepository.GetByIdAsync(BookingStatusHistoryId.Create(id), cancellationToken);
    }

    // Retorna todas las transiciones de estado sin filtro
    public async Task<IReadOnlyCollection<BookingStatusHistory>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _bookingStatusHistoryRepository.ListAsync(cancellationToken);
    }

    // Actualiza un registro verificando que exista, luego recrea el agregado con los nuevos datos
    public async Task<BookingStatusHistory> UpdateAsync(int id, DateTime changeDate, string? observation, int idBooking, int idStatus, int idUser, CancellationToken cancellationToken = default)
    {
        var historyId = BookingStatusHistoryId.Create(id);
        var existing = await _bookingStatusHistoryRepository.GetByIdAsync(historyId, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"BookingStatusHistory with id '{id}' was not found.");

        var updated = BookingStatusHistory.Create(id, changeDate, observation, idBooking, idStatus, idUser);
        await _bookingStatusHistoryRepository.UpdateAsync(updated, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return updated;
    }

    // Elimina un registro por su ID, retorna false si no existe en lugar de lanzar excepción
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var historyId = BookingStatusHistoryId.Create(id);
        var existing = await _bookingStatusHistoryRepository.GetByIdAsync(historyId, cancellationToken);
        if (existing is null)
            return false;

        await _bookingStatusHistoryRepository.DeleteAsync(historyId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
