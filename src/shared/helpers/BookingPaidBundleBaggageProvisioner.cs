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
    // Los bundles Basic/Classic/Flex NO son tipos de equipaje. Cuando se necesita materializar equipaje incluido,
    // se usan los tipos reales: mano (1) y bodega (2).
    private const int BaggageTypeCarryOn = 1;
    private const int BaggageTypeChecked = 2;

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
    /// (solo tipos reales de equipaje). Idempotente: no hace nada si ya hay cualquier equipaje en el tiquete.
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

        try
        {
            // Basic: solo artículo personal (bolso) → no se materializa como equipaje.
            if (string.Equals(tier, "Basic", StringComparison.OrdinalIgnoreCase))
                return;

            // Classic/Flex: incluye mano (10kg) y bodega (23kg) por pasajero.
            var slots = Math.Max(1, passengerSlots);
            var totalCarryOn = decimal.Round(10m * slots, 2, MidpointRounding.AwayFromZero);
            var totalChecked = decimal.Round(23m * slots, 2, MidpointRounding.AwayFromZero);

            await new CreateBaggageUseCase(repo).ExecuteAsync(totalCarryOn, idTicket, BaggageTypeCarryOn, ct);
            await new CreateBaggageUseCase(repo).ExecuteAsync(totalChecked, idTicket, BaggageTypeChecked, ct);
        }
        catch (Exception)
        {
            // Cualquier inconsistencia: no bloquea emisión ni check-in.
        }
    }
}
