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
using SistemaDeGestionDeTicketsAereos.src.modules.person.Domain.valueObject;
using SistemaDeGestionDeTicketsAereos.src.modules.person.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.modules.user.Application.UseCases;
using SistemaDeGestionDeTicketsAereos.src.modules.user.Infrastructure.Repositories;
using SistemaDeGestionDeTicketsAereos.src.shared.helpers;
using Spectre.Console;

namespace SistemaDeGestionDeTicketsAereos.src.shared.ui.menus;

public class LoginMenu : IModuleUI
{
    public string Key => "LOGIN";
    public string Title => "Inicio de Sesión";

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        while (!AppState.IsAuthenticated)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[yellow]SISTEMA DE GESTIÓN DE TIQUETES AÉREOS[/]").Centered());
            AnsiConsole.Write(new Rule("[blue]INICIO DE SESIÓN[/]").Centered());

            var initialOption = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Seleccione una opción:")
                    .PageSize(5)
                    .AddChoices("1. Iniciar Sesión", "2. Registrarse", "0. Salir del Sistema")
            );

            switch (initialOption)
            {
                case "0. Salir del Sistema":
                    Environment.Exit(0);
                    return;
                case "2. Registrarse":
                    await RegisterAsync(cancellationToken);
                    break;
                default:
                    await ShowLoginScreenWithRecoveryAsync(cancellationToken);
                    break;
            }
        }
    }

    private static async Task ShowLoginScreenWithRecoveryAsync(CancellationToken ct)
    {
        var attempt = true;
        while (attempt)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[blue]INICIAR SESIÓN[/]").Centered());
            var username = AnsiConsole.Ask<string>("Usuario: ");
            AnsiConsole.MarkupLine("[grey]Nota: la contraseña debe tener al menos 8 caracteres.[/]");
            var password = AnsiConsole.Prompt(new TextPrompt<string>("Contraseña: ").Secret());

            using var context = DbContextFactory.Create();
            var loginUseCase = new LoginUseCase(new UserRepository(context));

            try
            {
                var user = await loginUseCase.ExecuteAsync(username, password, ct);
                AppState.IsAuthenticated = true;
                AppState.IdUser = user.Id.Value;
                AppState.IdUserRole = user.IdUserRole;
                AppState.CurrentUser = user.Username.Value;
                AppState.IdPerson = user.IdPerson;
                AnsiConsole.MarkupLine("\n[green]¡Acceso concedido! Iniciando sistema...[/]");
                await Task.Delay(1500, ct);
                return;
            }
            catch (UnauthorizedAccessException ex)
            {
                AnsiConsole.MarkupLine($"\n[red]{Markup.Escape(ex.Message)}[/]");
                var next = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("¿Qué deseas hacer?")
                        .PageSize(5)
                        .AddChoices(
                            "1. Intentar iniciar sesión de nuevo",
                            "2. Registrarme (nueva cuenta)",
                            "3. Volver al menú de inicio"
                        ));
                if (next.StartsWith("1.", StringComparison.Ordinal))
                    continue;
                if (next.StartsWith("2.", StringComparison.Ordinal))
                {
                    await RegisterAsync(ct);
                    attempt = false;
                }
                else
                    attempt = false;
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine(
                    $"\n[red]Error al conectar o consultar el sistema:[/] {Markup.Escape(ex.Message)}");
                var next = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("¿Qué deseas hacer?")
                        .PageSize(4)
                        .AddChoices("1. Reintentar", "2. Volver al menú de inicio"));
                if (next.StartsWith("2.", StringComparison.Ordinal))
                    attempt = false;
            }
        }
    }

    private static async Task RegisterAsync(CancellationToken ct)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("[yellow]REGISTRO DE NUEVO CLIENTE[/]").Centered());
        AnsiConsole.MarkupLine("[grey]Completa los siguientes datos para crear tu cuenta.[/]\n");

        // ── Paso 1: credenciales ──────────────────────────────────────────────
        AnsiConsole.MarkupLine("[bold]Datos de acceso[/]");
        var username = AnsiConsole.Ask<string>("Nombre de usuario:");
        AnsiConsole.MarkupLine("[grey]La contraseña debe tener al menos 8 caracteres.[/]");
        var password = AnsiConsole.Prompt(
            new TextPrompt<string>("Contraseña:")
                .Secret()
                .Validate(p => !string.IsNullOrEmpty(p) && p.Length >= 8
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]La contraseña debe tener al menos 8 caracteres[/]")));
        var confirm  = AnsiConsole.Prompt(new TextPrompt<string>("Confirmar contraseña:").Secret());

        if (password != confirm)
        {
            AnsiConsole.MarkupLine("\n[red]Las contraseñas no coinciden. Intenta de nuevo.[/]");
            await Task.Delay(2000, ct);
            return;
        }

        // ── Paso 2: datos personales ──────────────────────────────────────────
        AnsiConsole.MarkupLine("\n[bold]Datos personales[/]");
        var firstName = AnsiConsole.Ask<string>("Nombre:");
        var lastName  = AnsiConsole.Ask<string>("Apellido:");

        var birthDateStr = AnsiConsole.Ask<string>("Fecha de nacimiento (yyyy-MM-dd):");
        if (!DateOnly.TryParse(birthDateStr, out var birthDate))
        {
            AnsiConsole.MarkupLine("\n[red]Formato de fecha inválido. Use yyyy-MM-dd.[/]");
            await Task.Delay(2000, ct);
            return;
        }

        // ── Paso 3: selectores (tipo doc, género, país) ───────────────────────
        try
        {
            using var ctxCatalogs = DbContextFactory.Create();

            var docTypes = await new GetAllDocumentTypesUseCase(new DocumentTypeRepository(ctxCatalogs)).ExecuteAsync(ct);
            if (!docTypes.Any()) throw new InvalidOperationException("No hay tipos de documento registrados. Pide a un administrador que los configure.");
            var selDoc = AnsiConsole.Prompt(
                new SelectionPrompt<string>().Title("Tipo de documento:").PageSize(8)
                    .AddChoices(docTypes.Select(d => $"{d.Id.Value}. {d.Name.Value}")));
            var idDocumentType = int.Parse(selDoc.Split(new char[] { '.' })[0]);

            var documentNumber = AnsiConsole.Ask<string>("Número de documento:");

            var genders = await new GetAllGendersUseCase(new GenderRepository(ctxCatalogs)).ExecuteAsync(ct);
            if (!genders.Any()) throw new InvalidOperationException("No hay géneros registrados. Pide a un administrador que los configure.");
            var selGender = AnsiConsole.Prompt(
                new SelectionPrompt<string>().Title("Género:").PageSize(6)
                    .AddChoices(genders.Select(g => $"{g.Id.Value}. {g.Description.Value}")));
            var idGender = int.Parse(selGender.Split(new char[] { '.' })[0]);

            var countries = await new GetAllCountriesUseCase(new CountryRepository(ctxCatalogs)).ExecuteAsync(ct);
            if (!countries.Any()) throw new InvalidOperationException("No hay países registrados. Pide a un administrador que los configure.");
            countries = countries
                .OrderBy(c => c.Id.Value)
                .ToList();
            var selCountry = AnsiConsole.Prompt(
                new SelectionPrompt<string>().Title("País de residencia:").PageSize(10)
                    .AddChoices(countries.Select(c => $"{c.Id.Value}. {c.Name.Value}")));
            var idCountry = int.Parse(selCountry.Split(new char[] { '.' })[0]);

            // ── Paso 4: email y teléfono ──────────────────────────────────────
            AnsiConsole.MarkupLine("\n[bold]Datos de contacto[/]");
            var email = AnsiConsole.Ask<string>("Correo electrónico:");
            var phoneInput = AnsiConsole.Prompt(
                new TextPrompt<string>("Teléfono (Enter para omitir):")
                    .AllowEmpty())
                .Trim();
            string? phone = string.IsNullOrEmpty(phoneInput) ? null : phoneInput;

            // ── Paso 5: guardar Person (contexto propio para obtener ID) ──────
            int realPersonId;
            using (var ctxPerson = DbContextFactory.Create())
            {
                await new CreatePersonUseCase(new PersonRepository(ctxPerson))
                    .ExecuteAsync(firstName, lastName, birthDate, documentNumber, idDocumentType, idGender, idCountry, null, ct);
                await ctxPerson.SaveChangesAsync(ct);

                var saved = await new PersonRepository(ctxPerson)
                    .GetByDocumentNumberAsync(PersonDocumentNumber.Create(documentNumber).Value, ct);
                if (saved is null) throw new InvalidOperationException("No se pudo recuperar la persona recién creada.");
                realPersonId = saved.Id.Value;
            }

            // ── Paso 6: Customer + User + Email + Phone ───────────────────────
            using (var ctxRest = DbContextFactory.Create())
            {
                var regCliente = DateOnly.FromDateTime(DateTime.Today).AddDays(1);
                await new CreateCustomerUseCase(new CustomerRepository(ctxRest))
                    .ExecuteAsync(regCliente, realPersonId, true, ct);

                await new CreateUserUseCase(new UserRepository(ctxRest))
                    .ExecuteAsync(username, password, 2, realPersonId, true, ct);

                await new CreateCustomerEmailUseCase(new CustomerEmailRepository(ctxRest))
                    .ExecuteAsync(email, realPersonId, ct);

                if (phone is not null)
                    await new CreateCustomerPhoneUseCase(new CustomerPhoneRepository(ctxRest))
                        .ExecuteAsync(phone, realPersonId, ct);

                await ctxRest.SaveChangesAsync(ct);
            }

            AnsiConsole.MarkupLine($"\n[green]¡Registro exitoso! Bienvenido, [bold]{Markup.Escape(firstName)}[/].[/]");
            AnsiConsole.MarkupLine("[grey]Ya puedes iniciar sesión con tus credenciales.[/]");
        }
        catch (InvalidOperationException ex)
        {
            AnsiConsole.MarkupLine($"\n[red]{Markup.Escape(ex.Message)}[/]");
        }
        catch (Exception ex)
        {
            EntityPersistenceUiFeedback.Write(ex);
        }

        AnsiConsole.MarkupLine("\n[grey]Presiona cualquier tecla para continuar...[/]");
        Console.ReadKey();
    }
}
