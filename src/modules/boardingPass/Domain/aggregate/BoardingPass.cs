using SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.boardingPass.Domain.aggregate;

public sealed class BoardingPass
{
    public BoardingPassId Id { get; private set; }
    public BoardingPassCode Code { get; private set; }

    public int IdTicket { get; private set; }
    public int IdSeat { get; private set; }

    public BoardingPassGate Gate { get; private set; }
    public DateTime BoardingTime { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public int IdStatus { get; private set; }

    /// <summary>Nombre del pasajero tal como se muestra en el pase (denormalizado para consultas rápidas).</summary>
    public string PassengerFullName { get; private set; } = string.Empty;

    private BoardingPass(
        BoardingPassId id,
        BoardingPassCode code,
        int idTicket,
        int idSeat,
        BoardingPassGate gate,
        DateTime boardingTime,
        DateTime createdAt,
        int idStatus,
        string passengerFullName)
    {
        Id = id;
        Code = code;
        IdTicket = idTicket;
        IdSeat = idSeat;
        Gate = gate;
        BoardingTime = boardingTime;
        CreatedAt = createdAt;
        IdStatus = idStatus;
        PassengerFullName = passengerFullName ?? string.Empty;
    }

    public static BoardingPass Create(
        int id,
        string code,
        int idTicket,
        int idSeat,
        string gate,
        DateTime boardingTime,
        DateTime createdAt,
        int idStatus,
        string passengerFullName)
    {
        if (idTicket <= 0) throw new ArgumentException("IdTicket must be > 0.", nameof(idTicket));
        if (idSeat <= 0) throw new ArgumentException("IdSeat must be > 0.", nameof(idSeat));
        if (idStatus <= 0) throw new ArgumentException("IdStatus must be > 0.", nameof(idStatus));
        if (boardingTime == default) throw new ArgumentException("BoardingTime is required.", nameof(boardingTime));
        if (createdAt == default) throw new ArgumentException("CreatedAt is required.", nameof(createdAt));
        if (passengerFullName is not null && passengerFullName.Length > 200)
            throw new ArgumentException("Passenger name cannot exceed 200 characters.", nameof(passengerFullName));
        return new BoardingPass(
            BoardingPassId.Create(id),
            BoardingPassCode.Create(code),
            idTicket,
            idSeat,
            BoardingPassGate.Create(gate),
            boardingTime,
            createdAt,
            idStatus,
            passengerFullName?.Trim() ?? string.Empty);
    }

    public static BoardingPass CreateNew(
        string code,
        int idTicket,
        int idSeat,
        string gate,
        DateTime boardingTime,
        DateTime createdAt,
        int idStatus,
        string passengerFullName) =>
        Create(0, code, idTicket, idSeat, gate, boardingTime, createdAt, idStatus, passengerFullName);
}

