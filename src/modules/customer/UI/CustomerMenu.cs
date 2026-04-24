using SistemaDeGestionDeTicketsAereos.src.modules.country.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.country.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.customer.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.customer.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.customerEmail.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.customerEmail.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.customerPhone.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.customerPhone.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.documentType.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.documentType.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.gender.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.gender.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.shared.helpers;
using SistemaDeGestionDeTicketsAereos.src.shared.ui.menus;
using Spectre.Console;

namespace SistemaDeGestionDeTicketsAereos.src.modules.customer.UI;

public sealed class CustomerMenu
{
    public async Task RunAsync(CancellationToken ct = default)
    {
        bool isAdmin = AppState.IdUserRole == 1;
        bool back = false;
        while (!back)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[green]GESTIÓN DE CLIENTES[/]").Centered());

            if (isAdmin)
            {
                var option = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .PageSize(9)
                        .AddChoices("1. Registrar cliente", "2. Listar clientes",
                                    "3. Actualizar cliente", "4. Eliminar cliente",
                                    "5. Gestionar emails", "6. Gestionar teléfonos", "0. Volver"));
                switch (option)
                {
                    case "1. Registrar cliente": await AdminCreateAsync(ct); break;
                    case "2. Listar clientes": await AdminListAsync(ct); break;
                    case "3. Actualizar cliente": await AdminUpdateAsync(ct); break;
                    case "4. Eliminar cliente": await DeleteAsync(ct); break;
                    case "5. Gestionar emails": await AdminEmailMenuAsync(ct); break;
                    case "6. Gestionar teléfonos": await AdminPhoneMenuAsync(ct); break;
                    case "0. Volver": back = true; break;
                }
            }
            else
            {
                // Vista simplificada para el cliente: solo sus propios datos
                AnsiConsole.MarkupLine($"[grey]Sesión: [bold]{Markup.Escape(AppState.CurrentUser ?? "")}[/][/]\n");
                var option = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .PageSize(5)
                        .AddChoices("1. Ver mi perfil", "2. Actualizar mi email",
                                    "3. Actualizar mi teléfono", "0. Volver"));
                switch (option)
                {
                    case "1. Ver mi perfil": await ViewMyProfileAsync(ct); break;
                    case "2. Actualizar mi email": await UpdateMyEmailAsync(ct); break;
                    case "3. Actualizar mi teléfono": await UpdateMyPhoneAsync(ct); break;
                    case "0. Volver": back = true; break;
                }
            }
        }
    }

    // ═══════════════════════════════════════════════════════════
    //  VISTAS CLIENTE
    // ═══════════════════════════════════════════════════════════

    private static async Task ViewMyProfileAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]MI PERFIL[/]").Centered());

        if (AppState.IdPerson is null)
        {
            AnsiConsole.MarkupLine("[yellow]Tu cuenta no tiene un perfil personal asociado. Contacta al administrador.[/]");
            ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false); return;
        }

        try
        {
            using var ctx = DbContextFactory.Create();
            var persons = await new GetAllPersonsUseCase(new PersonRepository(ctx)).ExecuteAsync(ct);
            var person = persons.FirstOrDefault(p => p.Id.Value == AppState.IdPerson);

            var customers = await new GetAllCustomersUseCase(new CustomerRepository(ctx)).ExecuteAsync(ct);
            var customer = customers.FirstOrDefault(c => c.IdPerson == AppState.IdPerson);

            var emails = await new GetAllCustomerEmailsUseCase(new CustomerEmailRepository(ctx)).ExecuteAsync(ct);
            var myEmail = emails.FirstOrDefault(e => e.IdPerson == AppState.IdPerson);

            var phones = await new GetAllCustomerPhonesUseCase(new CustomerPhoneRepository(ctx)).ExecuteAsync(ct);
            var myPhone = phones.FirstOrDefault(p => p.IdPerson == AppState.IdPerson);

            var docTypes = await new GetAllDocumentTypesUseCase(new DocumentTypeRepository(ctx)).ExecuteAsync(ct);
            var genders = await new GetAllGendersUseCase(new GenderRepository(ctx)).ExecuteAsync(ct);
            var docTypeMap = docTypes.ToDictionary(d => d.Id.Value, d => d.Name.Value);
            var genderMap = genders.ToDictionary(g => g.Id.Value, g => g.Description.Value);

            if (person is null)
            {
                AnsiConsole.MarkupLine("[yellow]No se encontró el perfil personal.[/]");
                ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false); return;
            }

            var docType = docTypeMap.TryGetValue(person.IdDocumentType, out var dt) ? dt : person.IdDocumentType.ToString();
            var gender = genderMap.TryGetValue(person.IdGender, out var gn) ? gn : person.IdGender.ToString();

            AnsiConsole.MarkupLine($"  [bold]Nombre:[/]       {Markup.Escape(person.FirstName.Value)} {Markup.Escape(person.LastName.Value)}");
            AnsiConsole.MarkupLine($"  [bold]Documento:[/]    {Markup.Escape(docType)} — {Markup.Escape(person.DocumentNumber.Value)}");
            AnsiConsole.MarkupLine($"  [bold]Nacimiento:[/]   {person.BirthDate.Value:yyyy-MM-dd}");
            AnsiConsole.MarkupLine($"  [bold]Género:[/]       {Markup.Escape(gender)}");
            AnsiConsole.MarkupLine($"  [bold]Usuario:[/]      {Markup.Escape(AppState.CurrentUser ?? "")}");
            if (customer is not null)
                AnsiConsole.MarkupLine($"  [bold]Registro:[/]     {customer.RegistrationDate.Value:yyyy-MM-dd}  Estado: {(customer.Active ? "[green]Activo[/]" : "[red]Inactivo[/]")}");
            AnsiConsole.MarkupLine($"  [bold]Email:[/]        {(myEmail is not null ? Markup.Escape(myEmail.Email.Value) : "[grey]Sin email registrado[/]")}");
            AnsiConsole.MarkupLine($"  [bold]Teléfono:[/]     {(myPhone is not null ? Markup.Escape(myPhone.Phone.Value) : "[grey]Sin teléfono registrado[/]")}");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }

        ConsolaPausa.PresionarCualquierTecla();
    }

    private static async Task UpdateMyEmailAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ACTUALIZAR MI EMAIL[/]").Centered());

        if (AppState.IdPerson is null)
        {
            AnsiConsole.MarkupLine("[yellow]Tu cuenta no tiene perfil asociado. Contacta al administrador.[/]");
            ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false); return;
        }

        try
        {
            using var ctx = DbContextFactory.Create();
            var emails = await new GetAllCustomerEmailsUseCase(new CustomerEmailRepository(ctx)).ExecuteAsync(ct);
            var myEmail = emails.FirstOrDefault(e => e.IdPerson == AppState.IdPerson);

            if (myEmail is not null)
            {
                AnsiConsole.MarkupLine($"Email actual: [bold]{Markup.Escape(myEmail.Email.Value)}[/]");
                var newEmail = AnsiConsole.Ask<string>("Nuevo email:");
                using var wCtx = DbContextFactory.Create();
                await new UpdateCustomerEmailUseCase(new CustomerEmailRepository(wCtx))
                    .ExecuteAsync(myEmail.Id.Value, newEmail, AppState.IdPerson.Value, ct);
                await wCtx.SaveChangesAsync(ct);
                AnsiConsole.MarkupLine("\n[green]Email actualizado correctamente.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[grey]No tienes email registrado aún.[/]");
                var newEmail = AnsiConsole.Ask<string>("Ingresa tu email:");
                using var wCtx = DbContextFactory.Create();
                await new CreateCustomerEmailUseCase(new CustomerEmailRepository(wCtx))
                    .ExecuteAsync(newEmail, AppState.IdPerson.Value, ct);
                await wCtx.SaveChangesAsync(ct);
                AnsiConsole.MarkupLine("\n[green]Email registrado correctamente.[/]");
            }
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task UpdateMyPhoneAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ACTUALIZAR MI TELÉFONO[/]").Centered());

        if (AppState.IdPerson is null)
        {
            AnsiConsole.MarkupLine("[yellow]Tu cuenta no tiene perfil asociado. Contacta al administrador.[/]");
            ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false); return;
        }

        try
        {
            using var ctx = DbContextFactory.Create();
            var phones = await new GetAllCustomerPhonesUseCase(new CustomerPhoneRepository(ctx)).ExecuteAsync(ct);
            var myPhone = phones.FirstOrDefault(p => p.IdPerson == AppState.IdPerson);

            if (myPhone is not null)
            {
                AnsiConsole.MarkupLine($"Teléfono actual: [bold]{Markup.Escape(myPhone.Phone.Value)}[/]");
                var newPhone = AnsiConsole.Ask<string>("Nuevo teléfono:");
                using var wCtx = DbContextFactory.Create();
                await new UpdateCustomerPhoneUseCase(new CustomerPhoneRepository(wCtx))
                    .ExecuteAsync(myPhone.Id.Value, newPhone, AppState.IdPerson.Value, ct);
                await wCtx.SaveChangesAsync(ct);
                AnsiConsole.MarkupLine("\n[green]Teléfono actualizado correctamente.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[grey]No tienes teléfono registrado aún.[/]");
                var newPhone = AnsiConsole.Ask<string>("Ingresa tu teléfono:");
                using var wCtx = DbContextFactory.Create();
                await new CreateCustomerPhoneUseCase(new CustomerPhoneRepository(wCtx))
                    .ExecuteAsync(newPhone, AppState.IdPerson.Value, ct);
                await wCtx.SaveChangesAsync(ct);
                AnsiConsole.MarkupLine("\n[green]Teléfono registrado correctamente.[/]");
            }
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    // ═══════════════════════════════════════════════════════════
    //  VISTAS ADMIN — CLIENTES
    // ═══════════════════════════════════════════════════════════

    private static async Task WriteAdminCustomerFullDetailListAsync(CancellationToken ct)
    {
        using var ctx = DbContextFactory.Create();
        var customers = (await new GetAllCustomersUseCase(new CustomerRepository(ctx)).ExecuteAsync(ct))
            .OrderBy(c => c.Id.Value).ToList();
        var persons = await new GetAllPersonsUseCase(new PersonRepository(ctx)).ExecuteAsync(ct);
        var emails = await new GetAllCustomerEmailsUseCase(new CustomerEmailRepository(ctx)).ExecuteAsync(ct);
        var phones = await new GetAllCustomerPhonesUseCase(new CustomerPhoneRepository(ctx)).ExecuteAsync(ct);
        var docTypes = await new GetAllDocumentTypesUseCase(new DocumentTypeRepository(ctx)).ExecuteAsync(ct);
        var genders = await new GetAllGendersUseCase(new GenderRepository(ctx)).ExecuteAsync(ct);
        var countries = await new GetAllCountriesUseCase(new CountryRepository(ctx)).ExecuteAsync(ct);
        var docTypeMap = docTypes.ToDictionary(d => d.Id.Value, d => d.Name.Value);
        var genderMap = genders.ToDictionary(g => g.Id.Value, g => g.Description.Value);
        var countryMap = countries.ToDictionary(c => c.Id.Value, c => c.Name.Value);
        var personById = persons.ToDictionary(p => p.Id.Value);

        if (!customers.Any()) { AnsiConsole.MarkupLine("[yellow]No hay clientes registrados.[/]"); return; }

        foreach (var c in customers)
        {
            if (!personById.TryGetValue(c.IdPerson, out var person))
            {
                AnsiConsole.MarkupLine($"[yellow]Cliente Id {c.Id.Value}: no se encontró la persona (IdPerson {c.IdPerson}).[/]\n");
                continue;
            }

            var docType = docTypeMap.TryGetValue(person.IdDocumentType, out var dt) ? dt : person.IdDocumentType.ToString();
            var gender = genderMap.TryGetValue(person.IdGender, out var gn) ? gn : person.IdGender.ToString();
            var country = countryMap.TryGetValue(person.IdCountry, out var co) ? co : person.IdCountry.ToString();

            var emOrdered = emails.Where(e => e.IdPerson == c.IdPerson).OrderBy(e => e.Id.Value).ToList();
            var phOrdered = phones.Where(p => p.IdPerson == c.IdPerson).OrderBy(p => p.Id.Value).ToList();
            var emailsLine = emOrdered.Count > 0
                ? string.Join("; ", emOrdered.Select(e => $"Id [bold]{e.Id.Value}[/]: {Markup.Escape(e.Email.Value)}"))
                : "[grey]Sin email[/]";
            var phonesLine = phOrdered.Count > 0
                ? string.Join("; ", phOrdered.Select(p => $"Id [bold]{p.Id.Value}[/]: {Markup.Escape(p.Phone.Value)}"))
                : "[grey]Sin teléfono[/]";

            AnsiConsole.Write(new Rule($"[green]Cliente [bold]#{c.Id.Value}[/] · Persona [bold]#{c.IdPerson}[/][/]"));
            AnsiConsole.MarkupLine($"  [bold]Nombre(s) y apellido(s):[/] {Markup.Escape(person.FirstName.Value)} {Markup.Escape(person.LastName.Value)}");
            AnsiConsole.MarkupLine($"  [bold]Tipo y número de documento:[/] {Markup.Escape(docType)} — {Markup.Escape(person.DocumentNumber.Value)}");
            AnsiConsole.MarkupLine($"  [bold]Fecha de nacimiento:[/]      {person.BirthDate.Value:yyyy-MM-dd}");
            AnsiConsole.MarkupLine($"  [bold]Género:[/]                   {Markup.Escape(gender)}");
            AnsiConsole.MarkupLine($"  [bold]País (nacionalidad):[/]      {Markup.Escape(country)}");
            AnsiConsole.MarkupLine($"  [bold]Correo(s):[/]                {emailsLine}");
            AnsiConsole.MarkupLine($"  [bold]Teléfono(s):[/]              {phonesLine}");
            AnsiConsole.MarkupLine($"  [bold]Fecha registro (cliente):[/] {c.RegistrationDate.Value:yyyy-MM-dd}");
            AnsiConsole.MarkupLine($"  [bold]Cliente activo:[/]          {(c.Active ? "[green]Sí[/]" : "[red]No[/]")}");
            AnsiConsole.WriteLine();
        }
    }

    private static async Task AdminListAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[green]LISTADO DE CLIENTES[/]").Centered());
        AnsiConsole.MarkupLine("[grey]Datos del perfil (persona), contactos y registro en el sistema.[/]\n");
        await WriteAdminCustomerFullDetailListAsync(ct);
        ConsolaPausa.PresionarCualquierTecla();
    }

    private static async Task<int> SelectPersonAsync(CancellationToken ct)
    {
        using var ctx = DbContextFactory.Create();
        var items = await new GetAllPersonsUseCase(new PersonRepository(ctx)).ExecuteAsync(ct);
        if (!items.Any()) throw new InvalidOperationException("No hay personas registradas. Crea una primero.");
        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>().Title("Selecciona la persona:").PageSize(10)
                .AddChoices(items.Select(p => $"{p.Id.Value}. {p.FirstName.Value} {p.LastName.Value} ({p.DocumentNumber.Value})")));
        return int.Parse(selected.Split(new char[] { '.' })[0]);
    }

    private static async Task<int> SelectDocumentTypeAsync(CancellationToken ct)
    {
        using var ctx = DbContextFactory.Create();
        var items = await new GetAllDocumentTypesUseCase(new DocumentTypeRepository(ctx)).ExecuteAsync(ct);
        if (!items.Any()) throw new InvalidOperationException("No hay tipos de documento. Crea uno primero.");
        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>().Title("Tipo de documento:").PageSize(10)
                .AddChoices(items.Select(d => $"{d.Id.Value}. {d.Name.Value}")));
        return int.Parse(selected.Split(new char[] { '.' })[0]);
    }

    private static async Task<int> SelectGenderAsync(CancellationToken ct)
    {
        using var ctx = DbContextFactory.Create();
        var items = await new GetAllGendersUseCase(new GenderRepository(ctx)).ExecuteAsync(ct);
        if (!items.Any()) throw new InvalidOperationException("No hay géneros registrados. Crea uno primero.");
        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>().Title("Género:").PageSize(10)
                .AddChoices(items.Select(g => $"{g.Id.Value}. {g.Description.Value}")));
        return int.Parse(selected.Split(new char[] { '.' })[0]);
    }

    private static async Task<int> SelectCountryAsync(CancellationToken ct)
    {
        using var ctx = DbContextFactory.Create();
        var items = await new GetAllCountriesUseCase(new CountryRepository(ctx)).ExecuteAsync(ct);
        if (!items.Any()) throw new InvalidOperationException("No hay países registrados. Crea uno primero.");
        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>().Title("País de nacionalidad:").PageSize(10)
                .AddChoices(items.Select(c => $"{c.Id.Value}. {c.Name.Value} ({c.ISOCode.Value})")));
        return int.Parse(selected.Split(new char[] { '.' })[0]);
    }

    private static async Task AdminCreateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]REGISTRAR CLIENTE[/]").Centered());
        if (!AnsiConsole.Confirm("¿Deseas registrar un cliente?", true))
            return;
        try
        {
            var idPerson = await SelectPersonAsync(ct);
            var active = AnsiConsole.Confirm("¿Cliente activo?", true);
            var hoy = DateOnly.FromDateTime(DateTime.Today);
            var regStr = AnsiConsole.Prompt(
                new TextPrompt<string>($"Fecha de registro como cliente (yyyy-MM-dd, debe ser [bold]posterior[/] a {hoy:yyyy-MM-dd}):")
                    .DefaultValue(hoy.AddDays(1).ToString("yyyy-MM-dd"))
                    .Validate(s =>
                    {
                        if (!DateOnly.TryParseExact(s, "yyyy-MM-dd", out var d))
                            return ValidationResult.Error("[red]Formato inválido. Usa yyyy-MM-dd.[/]");
                        if (d <= hoy)
                            return ValidationResult.Error("[red]La fecha de registro debe ser posterior a la fecha de hoy.[/]");
                        return ValidationResult.Success();
                    }));
            var regDate = DateOnly.ParseExact(regStr, "yyyy-MM-dd");
            using var ctx = DbContextFactory.Create();
            var result = await new CreateCustomerUseCase(new CustomerRepository(ctx))
                .ExecuteAsync(regDate, idPerson, active, ct);
            await ctx.SaveChangesAsync(ct);

            var createdId = (await new GetAllCustomersUseCase(new CustomerRepository(ctx)).ExecuteAsync(ct))
                .Where(c => c.IdPerson == idPerson && c.Active == active)
                .OrderByDescending(c => c.Id.Value)
                .Select(c => c.Id.Value)
                .FirstOrDefault();

            AnsiConsole.MarkupLine($"\n[green]Cliente registrado con ID {createdId}.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task AdminUpdateAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ACTUALIZAR CLIENTE[/]").Centered());
        AnsiConsole.MarkupLine("[grey]Mismo formato que «Listar clientes». Indica el [bold]IdCustomer[/] del encabezado [bold]Cliente #…[/]. Luego podrás editar los datos de la persona y del registro como cliente (correo y teléfono tienen sus propios menús).[/]\n");
        await WriteAdminCustomerFullDetailListAsync(ct);
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del cliente a actualizar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;

        try
        {
            using var ctx = DbContextFactory.Create();
            var customer = await new GetCustomerByIdUseCase(new CustomerRepository(ctx)).ExecuteAsync(id, ct);
            var person = await new GetPersonByIdUseCase(new PersonRepository(ctx)).ExecuteAsync(customer.IdPerson, ct);

            Console.Clear();
            AnsiConsole.Write(new Rule("[yellow]DATOS A ACTUALIZAR[/]").Centered());
            AnsiConsole.MarkupLine($"[grey]Persona vinculada: Id [bold]{customer.IdPerson}[/].[/]\n");

            var firstName = AnsiConsole.Prompt(new TextPrompt<string>("Nombre(s):").DefaultValue(person.FirstName.Value));
            var lastName = AnsiConsole.Prompt(new TextPrompt<string>("Apellido(s):").DefaultValue(person.LastName.Value));
            var birthStr = AnsiConsole.Prompt(
                new TextPrompt<string>("Fecha de nacimiento (yyyy-MM-dd):")
                    .DefaultValue(person.BirthDate.Value.ToString("yyyy-MM-dd"))
                    .Validate(s => DateOnly.TryParseExact(s, "yyyy-MM-dd", out _)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Formato inválido. Usa yyyy-MM-dd.[/]")));
            var birthDate = DateOnly.ParseExact(birthStr, "yyyy-MM-dd");
            var docNumber = AnsiConsole.Prompt(new TextPrompt<string>("Número de documento:").DefaultValue(person.DocumentNumber.Value));

            var idDocType = await SelectDocumentTypeAsync(ct);
            var idGender = await SelectGenderAsync(ct);
            var idCountry = await SelectCountryAsync(ct);

            var hoy = DateOnly.FromDateTime(DateTime.Today);
            var defaultReg = customer.RegistrationDate.Value > hoy
                ? customer.RegistrationDate.Value
                : hoy.AddDays(1);
            var regStr = AnsiConsole.Prompt(
                new TextPrompt<string>($"Fecha de registro como cliente (yyyy-MM-dd, debe ser [bold]posterior[/] a {hoy:yyyy-MM-dd}):")
                    .DefaultValue(defaultReg.ToString("yyyy-MM-dd"))
                    .Validate(s =>
                    {
                        if (!DateOnly.TryParseExact(s, "yyyy-MM-dd", out var d))
                            return ValidationResult.Error("[red]Formato inválido. Usa yyyy-MM-dd.[/]");
                        if (d <= hoy)
                            return ValidationResult.Error("[red]La fecha de registro debe ser posterior a la fecha de hoy.[/]");
                        return ValidationResult.Success();
                    }));
            var regDate = DateOnly.ParseExact(regStr, "yyyy-MM-dd");
            var active = AnsiConsole.Confirm("¿Cliente activo?", customer.Active);

            await new UpdatePersonUseCase(new PersonRepository(ctx))
                .ExecuteAsync(customer.IdPerson, firstName, lastName, birthDate, docNumber, idDocType, idGender, idCountry, person.IdAddress, ct);
            await new UpdateCustomerUseCase(new CustomerRepository(ctx))
                .ExecuteAsync(id, regDate, customer.IdPerson, active, ct);
            await ctx.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine("\n[green]Cliente y datos de persona actualizados correctamente.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task DeleteAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ELIMINAR CLIENTE[/]").Centered());
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del cliente a eliminar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        if (!AnsiConsole.Confirm($"¿Confirma eliminar el cliente con ID {id}?"))
        { AnsiConsole.MarkupLine("[grey]Operación cancelada.[/]"); Console.ReadKey(); return; }
        try
        {
            using var ctx = DbContextFactory.Create();
            var deleted = await new DeleteCustomerUseCase(new CustomerRepository(ctx)).ExecuteAsync(id, ct);
            await ctx.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine(deleted ? "\n[green]Cliente eliminado correctamente.[/]" : "\n[yellow]No se encontró el cliente con ese ID.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    // ═══════════════════════════════════════════════════════════
    //  VISTAS ADMIN — EMAILS
    // ═══════════════════════════════════════════════════════════

    private static async Task AdminEmailMenuAsync(CancellationToken ct)
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[green]EMAILS DE CLIENTES[/]").Centered());
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(5)
                    .AddChoices("1. Agregar email", "2. Actualizar email", "3. Eliminar email", "0. Volver"));
            switch (option)
            {
                case "1. Agregar email": await AdminCreateEmailAsync(ct); break;
                case "2. Actualizar email": await AdminUpdateEmailAsync(ct); break;
                case "3. Eliminar email": await AdminDeleteEmailAsync(ct); break;
                case "0. Volver": back = true; break;
            }
        }
    }

    private static async Task AdminCreateEmailAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]AGREGAR EMAIL[/]").Centered());
        if (!AnsiConsole.Confirm("¿Deseas agregar un email?", true))
            return;
        try
        {
            var idPerson = await SelectPersonAsync(ct);
            var email = AnsiConsole.Ask<string>("Email:");
            using var ctx = DbContextFactory.Create();
            var result = await new CreateCustomerEmailUseCase(new CustomerEmailRepository(ctx))
                .ExecuteAsync(email, idPerson, ct);
            await ctx.SaveChangesAsync(ct);

            var createdId = (await new GetAllCustomerEmailsUseCase(new CustomerEmailRepository(ctx)).ExecuteAsync(ct))
                .Where(e => e.IdPerson == idPerson && e.Email.Value == email)
                .OrderByDescending(e => e.Id.Value)
                .Select(e => e.Id.Value)
                .FirstOrDefault();

            AnsiConsole.MarkupLine($"\n[green]Email registrado con ID {createdId}.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task AdminUpdateEmailAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ACTUALIZAR EMAIL[/]").Centered());
        AnsiConsole.MarkupLine("[grey]Consulta el ID del correo en «2. Listar clientes» del menú de clientes, si lo necesitas.[/]\n");
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del email a actualizar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        var newEmail = AnsiConsole.Ask<string>("Nuevo email:");
        try
        {
            var idPerson = await SelectPersonAsync(ct);
            using var ctx = DbContextFactory.Create();
            await new UpdateCustomerEmailUseCase(new CustomerEmailRepository(ctx))
                .ExecuteAsync(id, newEmail, idPerson, ct);
            await ctx.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine("\n[green]Email actualizado correctamente.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task AdminDeleteEmailAsync(CancellationToken ct)
    {
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del email a eliminar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        if (!AnsiConsole.Confirm($"¿Confirma eliminar el email con ID {id}?"))
        { AnsiConsole.MarkupLine("[grey]Operación cancelada.[/]"); Console.ReadKey(); return; }
        try
        {
            using var ctx = DbContextFactory.Create();
            var deleted = await new DeleteCustomerEmailUseCase(new CustomerEmailRepository(ctx)).ExecuteAsync(id, ct);
            await ctx.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine(deleted ? "\n[green]Email eliminado correctamente.[/]" : "\n[yellow]No se encontró.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    // ═══════════════════════════════════════════════════════════
    //  VISTAS ADMIN — TELÉFONOS
    // ═══════════════════════════════════════════════════════════

    private static async Task AdminPhoneMenuAsync(CancellationToken ct)
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[green]TELÉFONOS DE CLIENTES[/]").Centered());
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(5)
                    .AddChoices("1. Agregar teléfono", "2. Actualizar teléfono", "3. Eliminar teléfono", "0. Volver"));
            switch (option)
            {
                case "1. Agregar teléfono": await AdminCreatePhoneAsync(ct); break;
                case "2. Actualizar teléfono": await AdminUpdatePhoneAsync(ct); break;
                case "3. Eliminar teléfono": await AdminDeletePhoneAsync(ct); break;
                case "0. Volver": back = true; break;
            }
        }
    }

    private static async Task AdminCreatePhoneAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]AGREGAR TELÉFONO[/]").Centered());
        if (!AnsiConsole.Confirm("¿Deseas agregar un teléfono?", true))
            return;
        try
        {
            var idPerson = await SelectPersonAsync(ct);
            var phone = AnsiConsole.Ask<string>("Teléfono:");
            using var ctx = DbContextFactory.Create();
            var result = await new CreateCustomerPhoneUseCase(new CustomerPhoneRepository(ctx))
                .ExecuteAsync(phone, idPerson, ct);
            await ctx.SaveChangesAsync(ct);

            var createdId = (await new GetAllCustomerPhonesUseCase(new CustomerPhoneRepository(ctx)).ExecuteAsync(ct))
                .Where(p => p.IdPerson == idPerson && p.Phone.Value == phone)
                .OrderByDescending(p => p.Id.Value)
                .Select(p => p.Id.Value)
                .FirstOrDefault();

            AnsiConsole.MarkupLine($"\n[green]Teléfono registrado con ID {createdId}.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task AdminUpdatePhoneAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]ACTUALIZAR TELÉFONO[/]").Centered());
        AnsiConsole.MarkupLine("[grey]Consulta el ID del teléfono en «2. Listar clientes» del menú de clientes, si lo necesitas.[/]\n");
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del teléfono a actualizar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        var newPhone = AnsiConsole.Ask<string>("Nuevo teléfono:");
        try
        {
            var idPerson = await SelectPersonAsync(ct);
            using var ctx = DbContextFactory.Create();
            await new UpdateCustomerPhoneUseCase(new CustomerPhoneRepository(ctx))
                .ExecuteAsync(id, newPhone, idPerson, ct);
            await ctx.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine("\n[green]Teléfono actualizado correctamente.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }

    private static async Task AdminDeletePhoneAsync(CancellationToken ct)
    {
        var id = AnsiConsole.Prompt(
            new TextPrompt<int>("ID del teléfono a eliminar (0 = Volver):")
                .Validate(v => v >= 0 ? ValidationResult.Success() : ValidationResult.Error("[red]El ID no puede ser negativo[/]")));
        if (id == 0) return;
        if (!AnsiConsole.Confirm($"¿Confirma eliminar el teléfono con ID {id}?"))
        { AnsiConsole.MarkupLine("[grey]Operación cancelada.[/]"); Console.ReadKey(); return; }
        try
        {
            using var ctx = DbContextFactory.Create();
            var deleted = await new DeleteCustomerPhoneUseCase(new CustomerPhoneRepository(ctx)).ExecuteAsync(id, ct);
            await ctx.SaveChangesAsync(ct);
            AnsiConsole.MarkupLine(deleted ? "\n[green]Teléfono eliminado correctamente.[/]" : "\n[yellow]No se encontró.[/]");
        }
        catch (Exception ex) { EntityPersistenceUiFeedback.Write(ex); }
        ConsolaPausa.PresionarCualquierTecla(conLineaInicial: false);
    }
}
