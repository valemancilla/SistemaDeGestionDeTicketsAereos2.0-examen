using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.fare.Domain.aggregate;
using SistemaDeGestionDeTicketsAereos.src.modules.fare.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.shared.helpers;

/// <summary>Precios por clase de asiento definidos en <c>FareSeatClassPrice</c> al crear la tarifa del vuelo.</summary>
public static class FareSeatClassPricingHelper
{
    public static async Task<Dictionary<int, Dictionary<int, decimal>>> LoadSeatClassPricesByFareIdAsync(
        AppDbContext context,
        IReadOnlyCollection<int> fareIds,
        CancellationToken ct)
    {
        var ids = fareIds.Where(id => id > 0).Distinct().ToList();
        if (ids.Count == 0)
            return new Dictionary<int, Dictionary<int, decimal>>();

        var rows = await context.Set<FareSeatClassPriceEntity>()
            .AsNoTracking()
            .Where(r => ids.Contains(r.IdFare))
            .Select(r => new { r.IdFare, r.IdClase, r.Price })
            .ToListAsync(ct);

        return rows.GroupBy(r => r.IdFare)
            .ToDictionary(g => g.Key, g => g.ToDictionary(x => x.IdClase, x => x.Price));
    }

    private static bool LooksLikeEconomyClassName(string? name) =>
        !string.IsNullOrEmpty(name) &&
        Regex.IsMatch(name, "econ", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    /// <summary>
    /// Referencia «sin adicional»: fila cuyo nombre de clase parece Económica; si no, el menor precio entre clases;
    /// si no hay filas, el precio base de la tarifa.
    /// </summary>
    public static decimal GetReferenceEconomyPrice(
        Fare fare,
        IReadOnlyDictionary<int, decimal>? pricesByClass,
        IReadOnlyDictionary<int, string>? seatClassIdToName)
    {
        if (pricesByClass is null || pricesByClass.Count == 0)
            return fare.BasePrice.Value;

        if (seatClassIdToName is not null)
        {
            foreach (var (idClase, price) in pricesByClass)
            {
                if (seatClassIdToName.TryGetValue(idClase, out var nm) && LooksLikeEconomyClassName(nm))
                    return price;
            }
        }

        return pricesByClass.Values.Min();
    }

    public static decimal GetSeatClassTotalPrice(Fare fare, IReadOnlyDictionary<int, decimal>? pricesByClass, int idClase)
    {
        if (pricesByClass is not null && pricesByClass.TryGetValue(idClase, out var p))
            return p;
        return fare.BasePrice.Value;
    }

    public static string FormatPriceCopColombia(decimal amount)
    {
        var n = amount.ToString("N0", CultureInfo.GetCultureInfo("es-CO"))
            .Replace('\u00a0', ' ')
            .Trim();
        return "$" + n + " COP";
    }
}
