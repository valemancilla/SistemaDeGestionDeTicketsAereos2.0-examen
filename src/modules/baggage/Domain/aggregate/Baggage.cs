// El equipaje está asociado a un ticket específico y tiene un peso y tipo definidos
using SistemaDeGestionDeTicketsAereos.src.modules.baggage.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.baggage.Domain.aggregate;

// Agregado Baggage: encapsula las reglas de negocio de un equipaje
public class Baggage
{
    // ID del equipaje (Value Object)
    public BaggageId Id { get; private set; }

    // Peso del equipaje en kilogramos (Value Object con límite de 50 kg)
    public BaggageWeight Weight { get; private set; }

    // ID del ticket al que pertenece este equipaje
    public int IdTicket { get; private set; }

    // ID del tipo de equipaje (mano, bodega, especial...)
    public int IdBaggageType { get; private set; }

    // Constructor privado: solo se crea a través del método Create
    private Baggage(BaggageId id, BaggageWeight weight, int idTicket, int idBaggageType)
    {
        Id = id;
        Weight = weight;
        IdTicket = idTicket;
        IdBaggageType = idBaggageType;
    }

    // Método de fábrica para crear o reconstruir un equipaje desde la base de datos
    public static Baggage Create(int id, decimal weight, int idTicket, int idBaggageType)
    {
        // Regla: el equipaje debe estar asociado a un tiquete válido
        if (idTicket <= 0)
            throw new ArgumentException("IdTicket must be greater than 0.", nameof(idTicket));

        // Regla: el equipaje debe tener un tipo válido asignado (mano, bodega, etc.)
        if (idBaggageType <= 0)
            throw new ArgumentException("IdBaggageType must be greater than 0.", nameof(idBaggageType));

        // Regla: el peso es validado por su Value Object (debe ser mayor a cero)
        return new Baggage(
            BaggageId.Create(id),
            BaggageWeight.Create(weight),
            idTicket,
            idBaggageType
        );
    }

    // Método de fábrica para crear un equipaje nuevo (ID = 0, la BD lo asigna después)
    public static Baggage CreateNew(decimal weight, int idTicket, int idBaggageType) => Create(0, weight, idTicket, idBaggageType);
}
