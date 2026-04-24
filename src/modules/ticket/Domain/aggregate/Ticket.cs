// El tiquete es el documento que autoriza al pasajero a abordar el vuelo — es el producto final del sistema
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Domain.valueObject;

namespace SistemaDeGestionDeTicketsAereos.src.modules.ticket.Domain.aggregate;

// Agregado Ticket: encapsula las reglas de negocio de un tiquete aéreo emitido
public class Ticket
{
    // ID del tiquete (Value Object)
    public TicketId Id { get; private set; }

    // Código único del tiquete — se usa para identificarlo en aeropuertos y abordaje
    public TicketCode Code { get; private set; }

    // Fecha y hora de emisión del tiquete
    public TicketIssueDate IssueDate { get; private set; }

    // FK a la reserva que originó este tiquete
    public int IdBooking { get; private set; }

    // FK a la tarifa aplicada al momento de la emisión
    public int IdFare { get; private set; }

    // FK al estado actual del tiquete (emitido, usado, cancelado, etc.)
    public int IdStatus { get; private set; }

    // Constructor privado: solo se crea a través del método Create
    private Ticket(TicketId id, TicketCode code, TicketIssueDate issueDate,
        int idBooking, int idFare, int idStatus)
    {
        Id = id;
        Code = code;
        IssueDate = issueDate;
        IdBooking = idBooking;
        IdFare = idFare;
        IdStatus = idStatus;
    }

    // Método de fábrica para crear o reconstruir un tiquete desde la base de datos
    public static Ticket Create(int id, string code, DateTime issueDate,
        int idBooking, int idFare, int idStatus)
    {
        // Regla: el tiquete debe estar asociado a una reserva válida
        if (idBooking <= 0)
            throw new ArgumentException("IdBooking must be greater than 0.", nameof(idBooking));

        // Regla: el tiquete debe tener una tarifa aplicada válida
        if (idFare <= 0)
            throw new ArgumentException("IdFare must be greater than 0.", nameof(idFare));

        // Regla: el estado del tiquete debe ser una referencia válida
        if (idStatus <= 0)
            throw new ArgumentException("IdStatus must be greater than 0.", nameof(idStatus));

        // Tiquete nuevo: la emisión es exactamente en este instante; no recibir "now" de otra capa (cruce con un
        // segundo DateTime.Now aquí o en el VO daba falso "futuro" si el reloj del SO ajustó hacia atrás entre medias).
        if (id == 0)
        {
            var atIssue = DateTime.Now;
            return new Ticket(
                TicketId.Create(id),
                TicketCode.Create(code),
                TicketIssueDate.Create(atIssue, atIssue),
                idBooking,
                idFare,
                idStatus);
        }

        // Reconstrucción desde BD / actualización con datos ya validados en capa de aplicación cuando aplica.
        return new Ticket(
            TicketId.Create(id),
            TicketCode.Create(code),
            TicketIssueDate.FromStorage(issueDate),
            idBooking,
            idFare,
            idStatus);
    }

    /// <summary>Alta: la fecha de emisión se fija ahora en el dominio (un solo <see cref="DateTime.Now"/>).</summary>
    public static Ticket CreateNew(string code, int idBooking, int idFare, int idStatus) =>
        Create(0, code, default, idBooking, idFare, idStatus);
}
