namespace SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Domain.valueObject;

public readonly record struct BoardingPassGate(string Value)
{
    public static BoardingPassGate Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("La puerta de embarque no puede estar vacía.", nameof(value));
        var v = value.Trim().ToUpperInvariant();
        if (v.Length > 10)
            throw new ArgumentException("La puerta no puede exceder 10 caracteres.", nameof(value));
        return new BoardingPassGate(v);
    }
}
