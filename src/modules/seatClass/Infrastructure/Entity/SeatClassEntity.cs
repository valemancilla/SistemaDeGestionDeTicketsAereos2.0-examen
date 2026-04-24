// La clase de asiento define la categoría de confort de un asiento en el avión
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Infrastructure.Entity;

namespace SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Infrastructure.Entity;

// Entidad que representa la tabla SeatClass en la base de datos
// Ejemplos: Económica, Business, Primera Clase
public class SeatClassEntity
{
    // Clave primaria de la clase de asiento
    public int IdClase { get; set; }

    // Nombre de la clase (ej: Económica, Business, Primera Clase)
    public string ClassName { get; set; } = string.Empty;

    // Asientos que pertenecen a esta clase
    public ICollection<SeatEntity> Seats { get; set; } = new List<SeatEntity>();
}
