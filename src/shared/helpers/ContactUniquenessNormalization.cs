namespace SistemaDeGestionDeTicketsAereos.src.shared.helpers;

/// <summary>
/// Normaliza email y teléfono para comparar duplicados de forma coherente (mayúsculas, espacios, etc.).
/// </summary>
public static class ContactUniquenessNormalization
{
    public static string NormalizeEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email)) return string.Empty;
        return email.Trim().ToLowerInvariant();
    }

    public static string NormalizePhone(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone)) return string.Empty;
        return string.Concat(phone.Where(c => !char.IsWhiteSpace(c) && c != '-'));
    }
}
