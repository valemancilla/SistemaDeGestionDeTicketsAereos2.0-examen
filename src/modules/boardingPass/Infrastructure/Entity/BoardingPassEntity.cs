using SistemaDeGestionDeTicketsAereos.src.modules.seat.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Infrastructure.Entity;

public sealed class BoardingPassEntity
{
    public int IdBoardingPass { get; set; }
    public string Code { get; set; } = string.Empty;

    public int IdTicket { get; set; }
    public int IdSeat { get; set; }

    public string Gate { get; set; } = string.Empty;
    public DateTime BoardingTime { get; set; }
    public DateTime CreatedAt { get; set; }

    public int IdStatus { get; set; }

    public string PassengerFullName { get; set; } = string.Empty;

    public TicketEntity Ticket { get; set; } = null!;
    public SeatEntity Seat { get; set; } = null!;
    public SystemStatusEntity Status { get; set; } = null!;
}

