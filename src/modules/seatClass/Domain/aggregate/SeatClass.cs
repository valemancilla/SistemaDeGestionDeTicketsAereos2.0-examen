// La clase de asiento define el nivel de servicio del pasajero (Económica, Business, Primera Clase)
using SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.seatClass.Domain.aggregate;

// Agregado SeatClass: encapsula las reglas de negocio de una clase de asiento
public class SeatClass
{
    // ID de la clase de asiento (Value Object)
    public SeatClassId Id { get; private set; }

    // Nombre de la clase (ej: "Económica", "Business", "Primera Clase")
    public SeatClassName Name { get; private set; }

    // Constructor privado: solo se crea a través del método Create
    private SeatClass(SeatClassId id, SeatClassName name)
    {
        Id = id;
        Name = name;
    }

    // Método de fábrica para crear o reconstruir una clase de asiento desde la base de datos
    public static SeatClass Create(int id, string name)
    {
        // Regla: el nombre de la clase es validado por su Value Object (no vacío)
        return new SeatClass(
            SeatClassId.Create(id),
            SeatClassName.Create(name)
        );
    }

    // Método de fábrica para crear una clase nueva (ID = 0, la BD lo asigna después)
    public static SeatClass CreateNew(string name) => Create(0, name);
}
