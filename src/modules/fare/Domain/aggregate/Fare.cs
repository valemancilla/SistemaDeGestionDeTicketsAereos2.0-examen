// La tarifa define el precio base y el período de validez de una oferta de vuelo de una aerolínea
using SistemaDeGestionDeTicketsAereos.src.modules.fare.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.fare.Domain.aggregate;

// Agregado Fare: encapsula las reglas de negocio de una tarifa aérea
public class Fare
{
    // ID de la tarifa (Value Object)
    public FareId Id { get; private set; }

    // Nombre de la tarifa (ej: "Económica", "Business Plus")
    public FareName Name { get; private set; }

    // Precio base antes de impuestos y cargos adicionales
    public FareBasePrice BasePrice { get; private set; }

    // Fecha desde la que la tarifa empieza a estar vigente
    public FareValidFrom ValidFrom { get; private set; }

    // Fecha hasta la que la tarifa está vigente
    public FareValidTo ValidTo { get; private set; }

    // Fecha opcional de expiración anticipada (si la aerolínea decide retirarla antes)
    public FareExpirationDate ExpirationDate { get; private set; }

    // FK a la aerolínea que ofrece esta tarifa
    public int IdAirline { get; private set; }

    // Indica si la tarifa está actualmente disponible para venta
    public bool Active { get; private set; }

    // Constructor privado: solo se crea a través del método Create
    private Fare(FareId id, FareName name, FareBasePrice basePrice,
        FareValidFrom validFrom, FareValidTo validTo, FareExpirationDate expirationDate,
        int idAirline, bool active)
    {
        Id = id;
        Name = name;
        BasePrice = basePrice;
        ValidFrom = validFrom;
        ValidTo = validTo;
        ExpirationDate = expirationDate;
        IdAirline = idAirline;
        Active = active;
    }

    // Método de fábrica para crear o reconstruir una tarifa desde la base de datos
    public static Fare Create(int id, string name, decimal basePrice,
        DateOnly validFrom, DateOnly validTo, DateOnly? expirationDate,
        int idAirline, bool active)
    {
        // Regla: toda tarifa debe pertenecer a una aerolínea válida
        if (idAirline <= 0)
            throw new ArgumentException("IdAirline must be greater than 0.", nameof(idAirline));

        // Regla: la fecha de inicio no puede ser posterior a la fecha fin.
        // Permitimos validFrom == validTo para tarifas válidas un solo día (rango incluyente).
        if (validFrom > validTo)
            throw new ArgumentException("ValidFrom cannot be after ValidTo.", nameof(validFrom));

        // Regla: la fecha de expiración no puede ser anterior a la fecha de fin de vigencia
        if (expirationDate.HasValue && expirationDate.Value < validTo)
            throw new ArgumentException("ExpirationDate cannot be before ValidTo.", nameof(expirationDate));

        // Regla: precio base y nombre son validados por sus Value Objects
        return new Fare(
            FareId.Create(id),
            FareName.Create(name),
            FareBasePrice.Create(basePrice),
            FareValidFrom.Create(validFrom),
            FareValidTo.Create(validTo),
            FareExpirationDate.Create(expirationDate),
            idAirline,
            active
        );
    }

    // Método de fábrica para crear una tarifa nueva (ID = 0, la BD lo asigna después)
    public static Fare CreateNew(string name, decimal basePrice, DateOnly validFrom, DateOnly validTo, DateOnly? expirationDate, int idAirline, bool active)
        => Create(0, name, basePrice, validFrom, validTo, expirationDate, idAirline, active);
}
