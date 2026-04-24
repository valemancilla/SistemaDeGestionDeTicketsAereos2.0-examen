// La ruta conecta dos aeropuertos y define la distancia y duración estimada del trayecto
using SistemaDeGestionDeTicketsAereos.src.modules.route.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.route.Domain.aggregate;

// Agregado Route: encapsula las reglas de negocio de una ruta aérea entre dos aeropuertos
public class Route
{
    // ID de la ruta (Value Object)
    public RouteId Id { get; private set; }

    // Distancia en kilómetros entre origen y destino
    public RouteDistanceKm DistanceKm { get; private set; }

    // Tiempo de vuelo estimado — se representa como TimeOnly (horas y minutos)
    public RouteEstDuration EstDuration { get; private set; }

    // FK al aeropuerto de origen
    public int OriginAirport { get; private set; }

    // FK al aeropuerto de destino
    public int DestinationAirport { get; private set; }

    // Indica si la ruta está operativa actualmente
    public bool Active { get; private set; }

    // Constructor privado: solo se crea a través del método Create
    private Route(RouteId id, RouteDistanceKm distanceKm, RouteEstDuration estDuration,
        int originAirport, int destinationAirport, bool active)
    {
        Id = id;
        DistanceKm = distanceKm;
        EstDuration = estDuration;
        OriginAirport = originAirport;
        DestinationAirport = destinationAirport;
        Active = active;
    }

    // Método de fábrica para crear o reconstruir una ruta desde la base de datos
    public static Route Create(int id, decimal distanceKm, TimeOnly estDuration,
        int originAirport, int destinationAirport, bool active)
    {
        // Regla: el aeropuerto de origen debe ser una referencia válida
        if (originAirport <= 0)
            throw new ArgumentException("OriginAirport must be greater than 0.", nameof(originAirport));

        // Regla: el aeropuerto de destino debe ser una referencia válida
        if (destinationAirport <= 0)
            throw new ArgumentException("DestinationAirport must be greater than 0.", nameof(destinationAirport));

        // Regla: una ruta no puede tener el mismo aeropuerto de origen y destino
        if (originAirport == destinationAirport)
            throw new ArgumentException("Origin and destination airports must be different.");

        // Regla: distancia y duración son validadas por sus Value Objects (valores positivos)
        return new Route(
            RouteId.Create(id),
            RouteDistanceKm.Create(distanceKm),
            RouteEstDuration.Create(estDuration),
            originAirport,
            destinationAirport,
            active
        );
    }

    // Método de fábrica para crear una ruta nueva (ID = 0, la BD lo asigna después)
    public static Route CreateNew(decimal distanceKm, TimeOnly estDuration, int originAirport, int destinationAirport, bool active)
        => Create(0, distanceKm, estDuration, originAirport, destinationAirport, active);
}
