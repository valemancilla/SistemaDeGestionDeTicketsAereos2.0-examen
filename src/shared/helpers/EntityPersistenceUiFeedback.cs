using Microsoft.EntityFrameworkCore;
using Spectre.Console;

namespace SistemaDeGestionDeTicketsAereos.src.shared.helpers;

/// <summary>
/// Mensajes de consola coherentes cuando <see cref="DbContext.SaveChangesAsync"/> falla
/// (p. ej. clave foránea al eliminar o actualizar).
/// </summary>
public static class EntityPersistenceUiFeedback
{
    public static void Write(Exception ex)
    {
        if (ex is DbUpdateException)
        {
            AnsiConsole.MarkupLine("\n[red]No se pudo guardar el cambio en la base de datos.[/]");
            AnsiConsole.MarkupLine("Lo más habitual es que [bold]existen otros registros que dependen[/] de este (restricción de clave foránea).");
            AnsiConsole.MarkupLine("[grey]Elimina o modifica primero los datos relacionados desde los menús correspondientes e inténtalo de nuevo.[/]");
            var detail = ex.InnerException?.Message;
            if (!string.IsNullOrWhiteSpace(detail))
                AnsiConsole.MarkupLine($"[grey]{Markup.Escape(detail)}[/]");
            return;
        }

        AnsiConsole.MarkupLine($"\n[red]Error:[/] {Markup.Escape(ex.Message)}");
    }
}
