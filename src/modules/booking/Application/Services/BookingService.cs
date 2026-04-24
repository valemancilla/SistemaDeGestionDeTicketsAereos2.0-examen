// Implementación del servicio de reservas: coordina el repositorio y la unidad de trabajo
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.booking.Application.Services;

// Servicio de aplicación de reservas — orquesta los casos de uso sin lógica de dominio propia
public sealed class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IUnitOfWork _unitOfWork;

    // Inyección de dependencias: el repositorio y la unidad de trabajo llegan por constructor
    public BookingService(IBookingRepository bookingRepository, IUnitOfWork unitOfWork)
    {
        _bookingRepository = bookingRepository;
        _unitOfWork = unitOfWork;
    }

    // Crea una reserva nueva verificando que el código no esté duplicado antes de persistir
    public async Task<Booking> CreateAsync(string code, DateTime flightDate, DateOnly creationDate, int seatCount, string? observations, int idFlight, int idStatus, CancellationToken cancellationToken = default)
    {
        var existing = await _bookingRepository.GetByCodeAsync(code, cancellationToken);
        if (existing is not null)
            throw new InvalidOperationException($"Booking with code '{code}' already exists.");

        var entity = Booking.CreateNew(code, flightDate, creationDate, seatCount, observations, idFlight, idStatus);
        await _bookingRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    // Busca una reserva por ID delegando directamente al repositorio
    public Task<Booking?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _bookingRepository.GetByIdAsync(BookingId.Create(id), cancellationToken);
    }

    // Retorna todas las reservas sin filtro
    public async Task<IReadOnlyCollection<Booking>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _bookingRepository.ListAsync(cancellationToken);
    }

    // Actualiza una reserva verificando que exista, luego recrea el agregado con los nuevos datos
    public async Task<Booking> UpdateAsync(int id, string code, DateTime flightDate, DateOnly creationDate, int seatCount, string? observations, int idFlight, int idStatus, CancellationToken cancellationToken = default)
    {
        var bookingId = BookingId.Create(id);
        var existing = await _bookingRepository.GetByIdAsync(bookingId, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"Booking with id '{id}' was not found.");

        var updated = Booking.Create(id, code, flightDate, creationDate, seatCount, observations, idFlight, idStatus);
        await _bookingRepository.UpdateAsync(updated, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return updated;
    }

    // Elimina una reserva por su ID, retorna false si no existe en lugar de lanzar excepción
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var bookingId = BookingId.Create(id);
        var existing = await _bookingRepository.GetByIdAsync(bookingId, cancellationToken);
        if (existing is null)
            return false;

        await _bookingRepository.DeleteAsync(bookingId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
