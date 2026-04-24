using System.Globalization;

namespace SistemaDeGestionDeTicketsAereos.src.shared.helpers;

public static class ClientBookingCodeOwnership
{
    /// <summary>True si el código tiene forma BK + id de usuario + 4 dígitos (reservas creadas desde búsqueda de vuelos como cliente).</summary>
    public static bool CodeLooksLikeClientGeneratedBooking(string? code, int idUser)
    {
        if (string.IsNullOrWhiteSpace(code) || idUser < 1)
            return false;
        if (!code.StartsWith("BK", StringComparison.OrdinalIgnoreCase))
            return false;

        var rest = code.AsSpan(2);
        if (rest.Length < 5)
            return false;

        var rnd = rest[^4..];
        foreach (var c in rnd)
        {
            if (!char.IsDigit(c))
                return false;
        }

        var userDigits = rest[..^4];
        if (userDigits.IsEmpty)
            return false;
        foreach (var c in userDigits)
        {
            if (!char.IsDigit(c))
                return false;
        }

        if (!int.TryParse(userDigits.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var uid))
            return false;
        return uid == idUser;
    }
}
