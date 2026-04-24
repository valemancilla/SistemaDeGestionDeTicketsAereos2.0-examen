// El check-in confirma que el pasajero se presentó al vuelo, registrando canal, asiento y estado
using SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.CheckIn.Domain.aggregate;

// Agregado CheckIn: encapsula las reglas de negocio del proceso de check-in de un pasajero
public class CheckIn
{
    // ID del check-in (Value Object)
    public CheckInId Id { get; private set; }

    // Fecha y hora en que se realizó el check-in (Value Object)
    public CheckInDate Date { get; private set; }

    // ID del ticket del pasajero que hace el check-in
    public int IdTicket { get; private set; }

    // ID del canal por el que se hizo el check-in (web, mostrador, app...)
    public int IdChannel { get; private set; }

    // ID del asiento asignado al pasajero
    public int IdSeat { get; private set; }

    // ID del usuario que procesó el check-in
    public int IdUser { get; private set; }

    // ID del estado actual del check-in
    public int IdStatus { get; private set; }

    // Constructor privado: solo se crea a través del método Create
    private CheckIn(CheckInId id, CheckInDate date,
        int idTicket, int idChannel, int idSeat, int idUser, int idStatus)
    {
        Id = id;
        Date = date;
        IdTicket = idTicket;
        IdChannel = idChannel;
        IdSeat = idSeat;
        IdUser = idUser;
        IdStatus = idStatus;
    }

    // Método de fábrica que valida las reglas antes de crear el check-in
    public static CheckIn Create(int id, DateTime date,
        int idTicket, int idChannel, int idSeat, int idUser, int idStatus)
    {
        // Regla: el check-in debe estar asociado a un tiquete válido
        if (idTicket <= 0)
            throw new ArgumentException("IdTicket must be greater than 0.", nameof(idTicket));

        // Regla: el check-in debe realizarse a través de un canal válido (web, mostrador, app)
        if (idChannel <= 0)
            throw new ArgumentException("IdChannel must be greater than 0.", nameof(idChannel));

        // Regla: el check-in debe tener un asiento asignado válido
        if (idSeat <= 0)
            throw new ArgumentException("IdSeat must be greater than 0.", nameof(idSeat));

        // Regla: el check-in debe ser registrado por un usuario válido del sistema
        if (idUser <= 0)
            throw new ArgumentException("IdUser must be greater than 0.", nameof(idUser));

        // Regla: el estado del check-in debe ser una referencia válida
        if (idStatus <= 0)
            throw new ArgumentException("IdStatus must be greater than 0.", nameof(idStatus));

        var checkInDate = id == 0
            ? CheckInDate.Create(date)
            : CheckInDate.FromStorage(date);

        return new CheckIn(
            CheckInId.Create(id),
            checkInDate,
            idTicket,
            idChannel,
            idSeat,
            idUser,
            idStatus
        );
    }

    // Método de fábrica para crear un check-in nuevo (ID = 0, la BD lo asigna después)
    public static CheckIn CreateNew(DateTime date, int idTicket, int idChannel, int idSeat, int idUser, int idStatus) => Create(0, date, idTicket, idChannel, idSeat, idUser, idStatus);
}
