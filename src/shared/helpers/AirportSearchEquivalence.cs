using SistemaDeGestionDeTicketsAereos.src.modules.airport.Domain.aggregate;

namespace SistemaDeGestionDeTicketsAereos.src.shared.helpers;

/// <summary>
/// Agrupa códigos IATA que en catálogo pueden estar duplicados como aeropuertos distintos
/// pero que en la práctica el cliente trata como el mismo destino al buscar vuelos.
/// </summary>
public static class AirportSearchEquivalence
{
    private static readonly string[][] IataGroups =
    {
        new[] { "MDE", "MED" },
    };

    /// <summary>
    /// Devuelve los <see cref="Airport.Id"/> que deben considerarse equivalentes al elegir <paramref name="airportId"/> en «Buscar vuelos».
    /// </summary>
    public static HashSet<int> ExpandAirportIdsForMatching(IReadOnlyList<Airport> airports, int airportId)
    {
        var me = airports.FirstOrDefault(a => a.Id.Value == airportId);
        if (me is null)
            return new HashSet<int> { airportId };

        var iata = me.IATACode.Value.Trim();
        foreach (var group in IataGroups)
        {
            if (!group.Any(g => string.Equals(g, iata, StringComparison.OrdinalIgnoreCase)))
                continue;

            var set = new HashSet<int>();
            foreach (var ap in airports)
            {
                var code = ap.IATACode.Value.Trim();
                if (group.Any(g => string.Equals(g, code, StringComparison.OrdinalIgnoreCase)))
                    set.Add(ap.Id.Value);
            }

            if (set.Count > 0)
                return set;
        }

        return new HashSet<int> { airportId };
    }
}
