using SistemaDeGestionDeTicketsAereos.src.modules.baggage.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.baggage.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.shared.context;

namespace SistemaDeGestionDeTicketsAereos.src.shared.helpers;

/// <summary>
/// La tarifa Basic/Classic/Flex del cliente se guarda en <see cref="SistemaDeGestionDeTicketsAereos.src.modules.booking.Domain.aggregate.Booking.Observations"/>;
/// este helper materializa una fila de equipaje asociada al tiquete para que check-in y consultas usen la tabla <c>Baggage</c>.
/// </summary>
public static class BookingPaidBundleBaggageProvisioner
{
    /// <summary>Peso de referencia por pasajero (mano + bodega, alineado al texto comercial de búsqueda de vuelos).</summary>
    private const decimal ReferenceBundleKilogramsPerPassenger = 33m;

    private const int BaggageTypeBasicBundle = 4;
    private const int BaggageTypeClassicBundle = 5;
    private const int BaggageTypeFlexBundle = 6;

    /// <returns>Basic, Classic, Flex o null si no hay texto de bundle cliente.</returns>
    public static string? DetectClientFareBundleTier(string? observations)
    {
        if (string.IsNullOrWhiteSpace(observations))
            return null;

        var s = observations;
        if (s.Contains(" Flex ", StringComparison.OrdinalIgnoreCase)
            || s.Contains("Flex ref", StringComparison.OrdinalIgnoreCase))
            return "Flex";

        if (s.Contains(" Classic ", StringComparison.OrdinalIgnoreCase)
            || s.Contains("Classic ref", StringComparison.OrdinalIgnoreCase))
            return "Classic";

        if (s.Contains(" Basic ", StringComparison.OrdinalIgnoreCase)
            || s.Contains("Basic ref", StringComparison.OrdinalIgnoreCase))
            return "Basic";

        return null;
    }

    /// <summary>
    /// Si el tiquete no tiene equipaje y la reserva indica bundle Basic/Classic/Flex, crea un registro de equipaje
    /// (tipos semilla 4–6). Idempotente: no hace nada si ya hay cualquier equipaje en el tiquete.
    /// </summary>
    public static async Task TryProvisionIfNoBaggageAsync(
        AppDbContext context,
        int idTicket,
        string? bookingObservations,
        int passengerSlots,
        CancellationToken ct)
    {
        var repo = new BaggageRepository(context);
        var existing = await repo.ListByTicketAsync(idTicket, ct);
        if (existing.Count > 0)
            return;

        var tier = DetectClientFareBundleTier(bookingObservations);
        if (tier is null)
            return;

        var slots = Math.Max(1, passengerSlots);
        var totalWeight = decimal.Round(ReferenceBundleKilogramsPerPassenger * slots, 2, MidpointRounding.AwayFromZero);
        var idType = tier switch
        {
            "Basic" => BaggageTypeBasicBundle,
            "Classic" => BaggageTypeClassicBundle,
            "Flex" => BaggageTypeFlexBundle,
            _ => 0
        };

        if (idType == 0)
            return;

        try
        {
            await new CreateBaggageUseCase(repo).ExecuteAsync(totalWeight, idTicket, idType, ct);
        }
        catch (Exception)
        {
            // FK si los tipos 4–6 no existen en BD (migración pendiente) u otra inconsistencia: no bloquea emisión ni check-in.
        }
    }
}
