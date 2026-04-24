namespace SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Domain.valueObject;

public readonly record struct BoardingPassCode(string Value)
{
    public static BoardingPassCode Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Code cannot be empty.", nameof(value));
        var v = value.Trim().ToUpperInvariant();
        if (v.Length is < 6 or > 25)
            throw new ArgumentException("Code length must be between 6 and 25.", nameof(value));
        return new BoardingPassCode(v);
    }
}

