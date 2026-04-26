// Implementación del servicio de pasajeros de reserva: coordina el repositorio y la unidad de trabajo
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Application.Interfaces;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Domain.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.shared.contracts;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Application.Services;

// Servicio de aplicación de pasajeros — orquesta sin lógica de dominio propia
public sealed class BookingCustomerService : IBookingCustomerService
{
    private readonly IBookingCustomerRepository _bookingCustomerRepository;
    private readonly IUnitOfWork _unitOfWork;

    // Inyección de dependencias: el repositorio y la unidad de trabajo llegan por constructor
    public BookingCustomerService(IBookingCustomerRepository bookingCustomerRepository, IUnitOfWork unitOfWork)
    {
        _bookingCustomerRepository = bookingCustomerRepository;
        _unitOfWork = unitOfWork;
    }

    // Asocia el pasajero a la reserva y lo persiste inmediatamente
    public async Task<BookingCustomer> CreateAsync(
        DateTime associationDate,
        int idBooking,
        int idUser,
        int idPerson,
        int idSeat,
        bool isPrimary,
        CancellationToken cancellationToken = default,
        bool isReadyToBoard = false)
    {
        var entity = BookingCustomer.CreateNew(associationDate, idBooking, idUser, idPerson, idSeat, isPrimary, isReadyToBoard);
        await _bookingCustomerRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    // Busca un pasajero por ID delegando directamente al repositorio
    public Task<BookingCustomer?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _bookingCustomerRepository.GetByIdAsync(BookingCustomerId.Create(id), cancellationToken);
    }

    // Retorna todos los pasajeros de reserva sin filtro
    public async Task<IReadOnlyCollection<BookingCustomer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _bookingCustomerRepository.ListAsync(cancellationToken);
    }

    // Actualiza un pasajero verificando que exista, luego recrea el agregado con los nuevos datos
    public async Task<BookingCustomer> UpdateAsync(
        int id,
        DateTime associationDate,
        int idBooking,
        int idUser,
        int idPerson,
        int idSeat,
        bool isPrimary,
        CancellationToken cancellationToken = default,
        bool isReadyToBoard = false)
    {
        var bookingCustomerId = BookingCustomerId.Create(id);
        var existing = await _bookingCustomerRepository.GetByIdAsync(bookingCustomerId, cancellationToken);
        if (existing is null)
            throw new KeyNotFoundException($"BookingCustomer with id '{id}' was not found.");

        var updated = BookingCustomer.Create(id, associationDate, idBooking, idUser, idPerson, idSeat, isPrimary, isReadyToBoard);
        await _bookingCustomerRepository.UpdateAsync(updated, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return updated;
    }

    // Elimina un pasajero por su ID, retorna false si no existe en lugar de lanzar excepción
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var bookingCustomerId = BookingCustomerId.Create(id);
        var existing = await _bookingCustomerRepository.GetByIdAsync(bookingCustomerId, cancellationToken);
        if (existing is null)
            return false;

        await _bookingCustomerRepository.DeleteAsync(bookingCustomerId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
