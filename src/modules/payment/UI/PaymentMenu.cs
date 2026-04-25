using Microsoft.EntityFrameworkCore;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Infrastructure.Entity;
using SistemaDeGestionDeTicketsAereos.src.modules.booking.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingCustomer.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.bookingStatusHistory.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.payment.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.payment.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.paymentmethod.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.systemStatus.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.ticket.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.payment.Application.Services;
using SistemaDeGestionDeTicketsAereos.src.shared.context;
using SistemaDeGestionDeTicketsAereos.src.shared.helpers;
using SistemaDeGestionDeTicketsAereos.src.shared.ui.menus;
using Spectre.Console;

namespace SistemaDeGestionDeTicketsAereos.src.modules.payment.UI;

public sealed class PaymentMenu
{
    private const string BookingEntityType = "Booking";
    private const string BookingStatusConfirmed = "Confirmada";
    private const string BookingStatusPaid = "Pagada";
    private const string PaymentEntityType = "Payment";
    private const string PaymentStatusApproved = "Aprobado";
    private const decimal MaxMontoPagoCop = 15_000_000m;

    private static ValidationResult ValidateMontoPago(decimal v)
    {
        if (v <= 0)
            return ValidationResult.Error("[red]El monto debe ser mayor a 0[/]");
        if (v > MaxMontoPagoCop)
            return ValidationResult.Error($"[red]El monto no puede superar {MaxMontoPagoCop:N0} COP[/]");
        return ValidationResult.Success();
    }

    public async Task RunAsync(CancellationToken ct = default)
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[green]GESTIÓN DE PAGOS[/]").Centered());
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(7)
                    .AddChoices("1. Registrar pago", "2. Listar pagos", "3. Actualizar pago",
                                "4. Eliminar pago", "0. Volver"));

            switch (option)
            {
                case "1. Registrar pago": await CreateAsync(ct); break;
                case "2. Listar pagos": await ListAsync(ct); break;
                case "3. Actualizar pago": await UpdateAsync(ct); break;
                case "4. Eliminar pago": await DeleteAsync(ct); break;
                case "0. Volver": back = true; break;
            }
        }
    }

    private static async Task ListAsync(CancellationToken ct)
    {
        Console.Clear();
        using var context = DbContextFactory.Create();
        var payments = await new GetAllPaymentsUseCase(new PaymentRepository(context)).ExecuteAsync(ct);
        var methods = await new GetAllPaymentMethodsUseCase(new PaymentMethodRepository(context)).ExecuteAsync(ct);
        var statuses = await new GetAllSystemStatusesUseCase(new SystemStatusRepository(context)).ExecuteAsync(ct);
        var methodMap = methods.ToDictionary(m => m.Id.Value, m => m.Name.Value);
        var statusMap = statuses.ToDictionary(s => s.Id.Value, s => s.Name.Value);

        if (AppState.IdUserRole != 1)
        {
            var myBookingIds = await GetMyBookingIdsAsync(ct);
            var paidBookingIds = await GetMyPaidBookingIdsAsync(context, myBookingIds, ct);
            payments = payments.Where(p => paidBookingIds.Contains(p.IdBooking)).ToList();
        }

        if (!payments.Any())
        {
            AnsiConsole.MarkupLine(
                AppState.IdUserRole != 1
                    ? "[yellow]No hay pagos asociados a [bold]reservas tuyas ya pagadas[/] (o aún no completaste el pago de la reserva).[/]"
                    : "[yellow]No hay pagos registrados.[/]");
        }
        else
        {
            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("ID"); table.AddColumn("Monto"); table.AddColumn("Fecha");
            table.AddColumn("Reserva ID"); table.AddColumn("Tiquete ID"); table.AddColumn("Método"); table.AddColumn("Estado");
            foreach (var p in payments)
            {
                var method = methodMap.TryGetValue(p.IdPaymentMethod, out var mn) ? mn : p.IdPaymentMethod.ToString();
                var status = statusMap.TryGetValue(p.IdStatus, out var sn) ? sn : p.IdStatus.ToString();
                var ticketCol = p.IdTicket.HasValue ? p.IdTicket.Value.ToString() : "—";
                table.AddRow(p.Id.Value.ToString(), p.Amount.Value.ToString("C2"),
                    p.Date.Value.ToString("yyyy-MM-dd HH:mm"),
                    p.IdBooking.ToString(), ticketCol, Markup.Escape(method), Markup.Escape(status));
            }
            AnsiConsole.Write(table);
        }
        ConsolaPausa.PresionarCualquierTecla();
    }

    private static async Task<int> SelectBookingAsync(CancellationToken ct)
    {
        using var context = DbContextFactory.Create();
        var bookings = await new GetAllBookingsUseCase(new BookingRepository(context)).ExecuteAsync(ct);
        if (AppState.IdUserRole != 1)
        {
            var myBookingIds = await GetMyBookingIdsAsync(ct);
            bookings = bookings.Where(b => myBookingIds.Contains(b.Id.Value)).ToList();
        }
        if (!bookings.Any()) throw new InvalidOperationException("No hay reservas registradas.");
        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>().Title("Selecciona la reserva:").PageSize(10)
                .AddChoices(bookings.Select(b => $"{b.Id.Value}. {b.Code.Value} — {b.FlightDate.Value:yyyy-MM-dd}")));
        return int.Parse(selected.Split(new char[] { '.' })[0]);
    }

    private static async Task<HashSet<int>> GetMyBookingIdsAsync(CancellationToken ct)
    {
        using var context = DbContextFactory.Create();
        var links = await new GetAllBookingCustomersUseCase(new BookingCustomerRepository(context)).ExecuteAsync(ct);
        var ids = new HashSet<int>();
        foreach (var l in links)
        {
            if (l.IdUser == AppState.IdUser)
                ids.Add(l.IdBooking);
            if (AppState.IdPerson is int myPersonId && l.IdPerson == myPersonId)
                ids.Add(l.IdBooking);
        }

        var all = await new GetAllBookingsUseCase(new BookingRepository(context)).ExecuteAsync(ct);
        foreach (var b in all)
        {
            if (ClientBookingCodeOwnership.CodeLooksLikeClientGeneratedBooking(b.Code.Value, AppState.IdUser))
                ids.Add(b.Id.Value);
        }

        return ids;
    }

    /// <summary>Reservas vinculadas al cliente cuyo estado de reserva es «Pagada» (compra finalizada).</summary>
    private static async Task<HashSet<int>> GetMyPaidBookingIdsAsync(AppDbContext context, HashSet<int> myBookingIds, CancellationToken ct)
    {
        var statuses = await new GetAllSystemStatusesUseCase(new SystemStatusRepository(context)).ExecuteAsync(ct);
        var paidStatus = statuses.FirstOrDefault(s =>
            string.Equals(s.EntityType.Value, BookingEntityType, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(s.Name.Value, BookingStatusPaid, StringComparison.OrdinalIgnoreCase));
        if (paidStatus is null)
            return new HashSet<int>();

        var allBookings = await new GetAllBookingsUseCase(new BookingRepository(context)).ExecuteAsync(ct);
        return allBookings
            .Where(b => b.IdStatus == paidStatus.Id.Value && myBookingIds.Contains(b.Id.Value))
            .Select(b => b.Id.Value)
            .ToHashSet();
    }

    private static async Task<int> GetStatusIdByNameAsync(string entityType, string statusName, CancellationToken ct)
    {
        using var context = DbContextFactory.Create();
        var statuses = await new GetAllSystemStatusesUseCase(new SystemStatusRepository(context)).ExecuteAsync(ct);
        var match = statuses.FirstOrDefault(s =>
            string.Equals(s.EntityType.Value, entityType, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(s.Name.Value, statusName, StringComparison.OrdinalIgnoreCase));
        if (match is null)
            throw new InvalidOperationException($"No existe el estado '{statusName}' para '{entityType}'. Revisa SystemStatus (Semillas).");
        return match.Id.Value;
    }

    private static readonly TicketIssuanceAfterPaymentService _ticketIssuance = new();

    private static async Task<int> SelectPaymentMethodAsync(CancellationToken ct)
    {
        using var context = DbContextFactory.Create();
        var methods = await new GetAllPaymentMethodsUseCase(new PaymentMethodRepository(context)).ExecuteAsync(ct);
        if (!methods.Any()) throw new InvalidOperationException("No hay métodos de pago. Crea uno en Administración.");
        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>().Title("Selecciona el método de pago:").PageSize(10)
                .AddChoices(methods.Select(m => $"{m.Id.Value}. {m.Name.Value}")));
        return int.Parse(selected.Split(new char[] { '.' })[0]);
    }

    private static async Task<int> SelectStatusAsync(CancellationToken ct)
    {
        using var context = DbContextFactory.Create();
        var statuses = await new GetAllSystemStatusesUseCase(new SystemStatusRepository(context)).ExecuteAsync(ct);
        var filtered = statuses.Where(s => s.EntityType.Value == "Payment").ToList();
        if (!filtered.Any()) throw new InvalidOperationException("No hay estados de pago. Crea uno en Administración (tipo: Payment).");
        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>().Title("Selecciona el estado del pago:").PageSize(10)
                .AddChoices(filtered.Select(s => $"{s.Id.Value}. {s.Name.Value}")));
        return int.Parse(selected.Split(new char[] { '.' })[0]);
    }

    private static async Task CreateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]REGISTRAR PAGO[/]").Centered());
        if (!AnsiConsole.Confirm("¿Deseas registrar un pago?", true))
            return;
        var amount = AnsiConsole.Prompt(
            new TextPrompt<decimal>("Monto del pago (COP, máx. 15.000.000):")
                .Validate(ValidateMontoPago));
        try
        {
            var idBooking = await SelectBookingAsync(ct);
            var idMethod = await SelectPaymentMethodAsync(ct);
            var idStatus = await SelectStatusAsync(ct);
            var idTicketRaw = AnsiConsole.Prompt(
                new TextPrompt<int>("ID de tiquete asociado (0 = ninguno):")
                    .DefaultValue(0)
                    .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]No puede ser negativo[/]")));
            int? idTicket = idTicketRaw > 0 ? idTicketRaw : null;

            using var context = DbContextFactory.Create();

            // Validación: si es cliente, el booking debe ser propio.
            if (AppState.IdUserRole != 1)
            {
                var myBookingIds = await GetMyBookingIdsAsync(ct);
                if (!myBookingIds.Contains(idBooking))
                    throw new InvalidOperationException("No puedes registrar pagos de una reserva que no te pertenece.");
            }

            var bookingRow = await context.Set<BookingEntity>().FirstOrDefaultAsync(b => b.IdBooking == idBooking, ct);
            if (bookingRow is null)
                throw new InvalidOperationException("No se encontró la reserva.");
            if (!bookingRow.ConsentDataProcessing || string.IsNullOrWhiteSpace(bookingRow.HolderEmail))
                throw new InvalidOperationException(
                    "Sin aceptar el tratamiento de datos del titular y sin correo de contacto guardado en la reserva no se puede registrar el pago. " +
                    "Completá titular y consentimiento en el flujo de [bold]Buscar vuelos[/] al armar la reserva (antes del pago).");

            if (idTicket is int tid)
            {
                var ticket = await new GetTicketByIdUseCase(new TicketRepository(context)).ExecuteAsync(tid, ct);
                if (ticket.IdBooking != idBooking)
                    throw new InvalidOperationException("El tiquete no pertenece a la reserva seleccionada.");
            }

            var result = await new CreatePaymentUseCase(new PaymentRepository(context))
                .ExecuteAsync(amount, DateTime.Now, idBooking, idMethod, idStatus, idTicket, ct);

            // Si el pago queda APROBADO, la reserva pasa a Pagada (flujo de compra cerrado).
            var approvedId = await GetStatusIdByNameAsync(PaymentEntityType, PaymentStatusApproved, ct);
            if (idStatus == approvedId)
            {
                var booking = await new GetBookingByIdUseCase(new BookingRepository(context)).ExecuteAsync(idBooking, ct);
                var canceledId = await GetStatusIdByNameAsync(BookingEntityType, "Cancelada", ct);
                if (booking.IdStatus == canceledId)
                    throw new InvalidOperationException("No se puede aprobar un pago para una reserva cancelada.");

                int targetBookingStatusId;
                string historyNote;
                try
                {
                    targetBookingStatusId = await GetStatusIdByNameAsync(BookingEntityType, BookingStatusPaid, ct);
                    historyNote = "Reserva en estado Pagada por pago aprobado.";
                }
                catch (InvalidOperationException)
                {
                    targetBookingStatusId = await GetStatusIdByNameAsync(BookingEntityType, BookingStatusConfirmed, ct);
                    historyNote = "Reserva confirmada por pago aprobado (estado Pagada no disponible en catálogo).";
                }

                if (booking.IdStatus != targetBookingStatusId)
                {
                    await new UpdateBookingUseCase(new BookingRepository(context))
                        .ExecuteAsync(
                            booking.Id.Value,
                            booking.Code.Value,
                            booking.FlightDate.Value,
                            booking.CreationDate.Value,
                            booking.SeatCount.Value,
                            booking.Observations.Value,
                            booking.IdFlight,
                            targetBookingStatusId,
                            ct);

                    await new CreateBookingStatusHistoryUseCase(new BookingStatusHistoryRepository(context))
                        .ExecuteAsync(DateTime.Now, historyNote, booking.Id.Value, targetBookingStatusId, AppState.IdUser, ct);
                }

                // Nuevo: al finalizar pago aprobado, emitir automáticamente los tiquetes de la reserva.
                await _ticketIssuance.EmitTicketsForBookingAsync(context, idBooking, ct);
            }

            await context.SaveChangesAsync(ct);

            var all = await new GetAllPaymentsUseCase(new PaymentRepository(context)).ExecuteAsync(ct);
            var createdId = all
                .Where(p =>
                    p.IdBooking == idBooking &&
                    p.IdPaymentMethod == idMethod &&
                    p.IdStatus == idStatus &&
                    p.Amount.Value == amount &&
                    p.IdTicket == idTicket)
                .OrderByDescending(p => p.Id.Value)
                .Select(p => p.Id.Value)
                .FirstOrDefault();

            AnsiConsole.MarkupLine($"\n[green]Pago registrado con ID {createdId} por ${result.Amount.Value:N2}.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task UpdateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ACTUALIZAR PAGO[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del pago a actualizar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        var amount = AnsiConsole.Prompt(
            new TextPrompt<decimal>("Nuevo monto (COP, máx. 15.000.000):")
                .Validate(ValidateMontoPago));
        try
        {
            using var pre = DbContextFactory.Create();
            var existingPay = await new GetPaymentByIdUseCase(new PaymentRepository(pre)).ExecuteAsync(id, ct);

            var idBooking = await SelectBookingAsync(ct);
            var idMethod = await SelectPaymentMethodAsync(ct);
            var idStatus = await SelectStatusAsync(ct);
            var ticketStr = AnsiConsole.Prompt(
                new TextPrompt<string>("ID tiquete (vacío = sin cambio, 0 = quitar):")
                    .AllowEmpty());
            int? idTicket = existingPay.IdTicket;
            if (string.Equals(ticketStr.Trim(), "0", StringComparison.Ordinal))
                idTicket = null;
            else if (int.TryParse(ticketStr.Trim(), out var parsedTicket) && parsedTicket > 0)
                idTicket = parsedTicket;

            using var context = DbContextFactory.Create();
            if (idTicket is int tid2)
            {
                var ticket = await new GetTicketByIdUseCase(new TicketRepository(context)).ExecuteAsync(tid2, ct);
                if (ticket.IdBooking != idBooking)
                    throw new InvalidOperationException("El tiquete no pertenece a la reserva seleccionada.");
            }

            await new UpdatePaymentUseCase(new PaymentRepository(context))
                .ExecuteAsync(id, amount, DateTime.Now, idBooking, idMethod, idStatus, idTicket, ct);

            // Si el pago queda APROBADO, la reserva pasa a Pagada y se emiten tiquetes automáticamente.
            var approvedId = await GetStatusIdByNameAsync(PaymentEntityType, PaymentStatusApproved, ct);
            if (idStatus == approvedId)
            {
                var bookingRepo = new BookingRepository(context);
                var booking = await new GetBookingByIdUseCase(bookingRepo).ExecuteAsync(idBooking, ct);
                var canceledId = await GetStatusIdByNameAsync(BookingEntityType, "Cancelada", ct);
                if (booking.IdStatus != canceledId)
                {
                    int targetBookingStatusId;
                    string historyNote;
                    try
                    {
                        targetBookingStatusId = await GetStatusIdByNameAsync(BookingEntityType, BookingStatusPaid, ct);
                        historyNote = "Reserva en estado Pagada por pago aprobado.";
                    }
                    catch (InvalidOperationException)
                    {
                        targetBookingStatusId = await GetStatusIdByNameAsync(BookingEntityType, BookingStatusConfirmed, ct);
                        historyNote = "Reserva confirmada por pago aprobado (estado Pagada no disponible en catálogo).";
                    }

                    if (booking.IdStatus != targetBookingStatusId)
                    {
                        await new UpdateBookingUseCase(bookingRepo)
                            .ExecuteAsync(
                                booking.Id.Value,
                                booking.Code.Value,
                                booking.FlightDate.Value,
                                booking.CreationDate.Value,
                                booking.SeatCount.Value,
                                booking.Observations.Value,
                                booking.IdFlight,
                                targetBookingStatusId,
                                ct);
                        await new CreateBookingStatusHistoryUseCase(new BookingStatusHistoryRepository(context))
                            .ExecuteAsync(DateTime.Now, historyNote, booking.Id.Value, targetBookingStatusId, AppState.IdUser, ct);
                    }
                }

                await _ticketIssuance.EmitTicketsForBookingAsync(context, idBooking, ct);
            }
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine("\n[green]Pago actualizado correctamente.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task DeleteAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ELIMINAR PAGO[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del pago a eliminar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        if (!AnsiConsole.Confirm($"¿Confirma eliminar el pago con ID {id}?"))
        { AnsiConsole.MarkupLine("[grey]Operación cancelada.[/]"); Console.ReadKey(); return; }
        try
        {
            using var context = DbContextFactory.Create();
            var deleted = await new DeletePaymentUseCase(new PaymentRepository(context)).ExecuteAsync(id, ct);
            await context.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine(deleted ? "\n[green]Pago eliminado correctamente.[/]" : "\n[yellow]No se encontró el pago con ese ID.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }
}
