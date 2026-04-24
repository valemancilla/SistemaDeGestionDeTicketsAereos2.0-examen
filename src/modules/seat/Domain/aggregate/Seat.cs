// El asiento es un lugar físico específico dentro de una aeronave, identificado por número y clase
using SistemaDeGestionDeTicketsAereos.src.modules.seat.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.seat.Domain.aggregate;

// Agregado Seat: encapsula las reglas de negocio de un asiento de aeronave
public class Seat
{
    // ID del asiento (Value Object)
    public SeatId Id { get; private set; }

    // Número del asiento en formato estándar (ej: "12A", "1C", "34B")
    public SeatNumber Number { get; private set; }

    // FK a la aeronave a la que pertenece este asiento
    public int IdAircraft { get; private set; }

    // FK a la clase del asiento (Económica, Business, Primera Clase)
    public int IdClase { get; private set; }

    // Constructor privado: solo se crea a través del método Create
    private Seat(SeatId id, SeatNumber number, int idAircraft, int idClase)
    {
        Id = id;
        Number = number;
        IdAircraft = idAircraft;
        IdClase = idClase;
    }

    // Método de fábrica para crear o reconstruir un asiento desde la base de datos
    public static Seat Create(int id, string number, int idAircraft, int idClase)
    {
        // Regla: todo asiento debe pertenecer a una aeronave válida
        if (idAircraft <= 0)
            throw new ArgumentException("IdAircraft must be greater than 0.", nameof(idAircraft));

        // Regla: todo asiento debe tener una clase asignada (económica, business, etc.)
        if (idClase <= 0)
            throw new ArgumentException("IdClase must be greater than 0.", nameof(idClase));

        // Regla: el número de asiento es validado por su Value Object (formato ej. 12A)
        return new Seat(
            SeatId.Create(id),
            SeatNumber.Create(number),
            idAircraft,
            idClase
        );
    }

    // Método de fábrica para crear un asiento nuevo (ID = 0, la BD lo asigna después)
    public static Seat CreateNew(string number, int idAircraft, int idClase) => Create(0, number, idAircraft, idClase);
}
