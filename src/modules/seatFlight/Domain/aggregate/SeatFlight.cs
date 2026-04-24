// La asignación asiento-vuelo controla qué asientos están disponibles para un vuelo en particular
using SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.seatFlight.Domain.aggregate;

// Agregado SeatFlight: encapsula la relación entre un asiento y un vuelo específico
public class SeatFlight
{
    // ID de la asignación (Value Object)
    public SeatFlightId Id { get; private set; }

    // FK al asiento de la aeronave
    public int IdSeat { get; private set; }

    // FK al vuelo en que opera ese asiento
    public int IdFlight { get; private set; }

    // Indica si el asiento sigue disponible para ser reservado en ese vuelo
    public bool Available { get; private set; }

    // Constructor privado: solo se crea a través del método Create
    private SeatFlight(SeatFlightId id, int idSeat, int idFlight, bool available)
    {
        Id = id;
        IdSeat = idSeat;
        IdFlight = idFlight;
        Available = available;
    }

    // Método de fábrica para crear o reconstruir una asignación asiento-vuelo desde la base de datos
    public static SeatFlight Create(int id, int idSeat, int idFlight, bool available)
    {
        // Regla: el asiento asociado debe ser una referencia válida
        if (idSeat <= 0)
            throw new ArgumentException("IdSeat must be greater than 0.", nameof(idSeat));

        // Regla: el vuelo asociado debe ser una referencia válida
        if (idFlight <= 0)
            throw new ArgumentException("IdFlight must be greater than 0.", nameof(idFlight));

        return new SeatFlight(
            SeatFlightId.Create(id),
            idSeat,
            idFlight,
            available
        );
    }

    // Método de fábrica para crear una asignación nueva (ID = 0, la BD lo asigna después)
    public static SeatFlight CreateNew(int idSeat, int idFlight, bool available) => Create(0, idSeat, idFlight, available);
}
