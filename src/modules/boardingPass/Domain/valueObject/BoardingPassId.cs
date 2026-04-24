namespace SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Domain.valueObject;

public readonly record struct BoardingPassId(int Value)
{
    public static BoardingPassId Create(int value) =>
        value < 0 ? throw new ArgumentException("Id must be >= 0.", nameof(value)) : new BoardingPassId(value);
}

