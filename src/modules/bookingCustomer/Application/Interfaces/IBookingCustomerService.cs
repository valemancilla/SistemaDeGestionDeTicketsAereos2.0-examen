// Contrato del servicio de pasajeros de reserva: define las operaciones disponibles para la capa de presentación
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Application.Interfaces;

// Interfaz del servicio de pasajeros — desacopla la capa UI del servicio concreto
public interface IBookingCustomerService
{
    // Asocia un pasajero a una reserva indicando si es el titular (isPrimary)
    Task<BookingCustomer> CreateAsync(DateTime associationDate, int idBooking, int idUser, int idPerson, int idSeat, bool isPrimary, CancellationToken cancellationToken = default, bool isReadyToBoard = false);

    // Busca un pasajero de reserva por su ID, retorna null si no existe
    Task<BookingCustomer?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    // Retorna todos los pasajeros asociados a reservas en el sistema
    Task<IReadOnlyCollection<BookingCustomer>> GetAllAsync(CancellationToken cancellationToken = default);

    // Actualiza los datos de un pasajero de reserva existente, lanza excepción si no se encuentra
    Task<BookingCustomer> UpdateAsync(int id, DateTime associationDate, int idBooking, int idUser, int idPerson, int idSeat, bool isPrimary, CancellationToken cancellationToken = default, bool isReadyToBoard = false);

    // Elimina un pasajero de reserva por su ID, retorna false si no existe
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
