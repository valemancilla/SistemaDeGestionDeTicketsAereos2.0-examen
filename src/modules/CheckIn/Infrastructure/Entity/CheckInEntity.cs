// El check-in registra que un pasajero se presentó al vuelo, por qué canal lo hizo y qué asiento tomó
using SistemaDeGestionDeTicketsAereos.src.modules.checkinChannel.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.user.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Infrastructure.Entity;

// Entidad que representa la tabla CheckIn en la base de datos
public class CheckInEntity
{
    // Clave primaria del check-in
    public int IdCheckIn { get; set; }

    // FK al ticket del pasajero que hace el check-in
    public int IdTicket { get; set; }

    // Fecha y hora en que se realizó el check-in
    public DateTime CheckInDate { get; set; }

    // FK al canal por el que se hizo el check-in (web, mostrador, app móvil...)
    public int IdChannel { get; set; }

    // FK al asiento que se le asignó al pasajero en el check-in
    public int IdSeat { get; set; }

    // FK al usuario que procesó el check-in
    public int IdUser { get; set; }

    // FK al estado actual del check-in
    public int IdStatus { get; set; }

    // Navegación al ticket del pasajero
    public TicketEntity Ticket { get; set; } = null!;

    // Navegación al canal de check-in utilizado
    public CheckInChannelEntity Channel { get; set; } = null!;

    // Navegación al asiento asignado
    public SeatEntity Seat { get; set; } = null!;

    // Navegación al usuario que procesó el check-in
    public UserEntity User { get; set; } = null!;

    // Navegación al estado del check-in
    public SystemStatusEntity Status { get; set; } = null!;
}
