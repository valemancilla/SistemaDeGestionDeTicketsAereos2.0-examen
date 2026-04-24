// Implementación del servicio de cancelaciones de reserva: coordina el repositorio y la unidad de trabajo
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingCancellation.Application.Services;

// Servicio de aplicación de cancelaciones — orquesta sin lógica de dominio propia
public sealed class BookingCancellationService : IBookingCancellationService
{
    private readonly IBookingCancellationRepository _bookingCancellationRepository;
    private readonly IUnitOfWork _unitOfWork;

    // Inyección de dependencias: el repositorio y la unidad de trabajo llegan por constructor
    public BookingCancellationService(IBookingCancellationRepository bookingCancellationRepository, IUnitOfWork unitOfWork)
    {
        _bookingCancellationRepository = bookingCancellationRepository;
        _unitOfWork = unitOfWork;
    }

    // Registra una cancelación nueva y la persiste inmediatamente
    public async Task<BookingCancellation> CreateAsync(DateTime cancellationDate, string reason, decimal penaltyAmount, int idBooking, int idUser, CancellationToken cancellationToken = default)
    {
        var entity = BookingCancellation.CreateNew(cancellationDate, reason, penaltyAmount, idBooking, idUser);
        await _bookingCancellationRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    // Busca una cancelación por ID delegando directamente al repositorio
    public Task<BookingCancellation?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _bookingCancellationRepository.GetByIdAsync(BookingCancellationId.Create(id), cancellationToken);
    }

    // Retorna todas las cancelaciones sin filtro
    public async Task<IReadOnlyCollection<BookingCancellation>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _bookingCancellationRepository.ListAsync(cancellationToken);
    }

    // Actualiza una cancelación verificando que exista, luego recrea el agregado con los nuevos datos
    public async Task<BookingCancellation> UpdateAsync(int id, DateTime cancellationDate, string reason, decimal penaltyAmount, int idBooking, int idUser, CancellationToken cancellationToken = default)
    {
        var cancellationId = BookingCancellationId.Create(id);
        var existing = await _bookingCancellationRepository.GetByIdAsync(cancellationId, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"BookingCancellation with id '{id}' was not found.");

        var updated = BookingCancellation.Create(id, cancellationDate, reason, penaltyAmount, idBooking, idUser);
        await _bookingCancellationRepository.UpdateAsync(updated, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return updated;
    }

    // Elimina una cancelación por su ID, retorna false si no existe en lugar de lanzar excepción
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var cancellationId = BookingCancellationId.Create(id);
        var existing = await _bookingCancellationRepository.GetByIdAsync(cancellationId, cancellationToken);
        if (existing is null)
            return false;

        await _bookingCancellationRepository.DeleteAsync(cancellationId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
