// Precio por clase de asiento asociado a una tarifa.
using SistemaDeGestionDeTicketsAereos.src.modules.fare.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.fare.Infrastructure.Entity;

public sealed class FareSeatClassPriceEntity
{
    public int IdFare { get; set; }
    public int IdClase { get; set; }

    public decimal Price { get; set; }

    public FareEntity Fare { get; set; } = null!;
    public SeatClassEntity SeatClass { get; set; } = null!;
}

