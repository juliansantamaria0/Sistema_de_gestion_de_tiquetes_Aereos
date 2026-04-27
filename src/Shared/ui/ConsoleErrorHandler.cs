using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Spectre.Console;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Infrastructure;

namespace Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public static class ConsoleErrorHandler
{
    private const int VarcharSafeMax = 60;

    public static async Task RunSafeAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default)
    {
        _ = await TryRunSafeAsync(action, cancellationToken).ConfigureAwait(false);
    }

    public static async Task<bool> TryRunSafeAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default)
    {
        try
        {
            await action(cancellationToken).ConfigureAwait(false);
            return true;
        }
        catch (FlowAbortException)
        {
            AnsiConsole.MarkupLine("[grey]Operación cancelada.[/]");
            Pause();
            return false;
        }
        catch (TargetInvocationException ex) when (ex.InnerException is DbUpdateException dbInner)
        {
            if (dbInner is DbUpdateConcurrencyException)
                AnsiConsole.MarkupLine("[red]El asiento ya fue confirmado por alguien más.[/]");
            else
                WriteFriendlyDbUpdateException(dbInner);
            Pause();
            return false;
        }
        catch (TargetInvocationException ex) when (ex.InnerException is not null)
        {
            AnsiConsole.MarkupLine($"[red]{Markup.Escape(TruncateMessage(ex.InnerException.Message))}[/]");
            Pause();
            return false;
        }
        catch (DbUpdateConcurrencyException)
        {
            AnsiConsole.MarkupLine("[red]El asiento ya fue confirmado por alguien más.[/]");
            Pause();
            return false;
        }
        catch (DbUpdateException ex)
        {
            WriteFriendlyDbUpdateException(ex);
            Pause();
            return false;
        }
        catch (MySqlException ex)
        {
            var msg = MySqlErrorFormatter.ToUserMessage(ex);
            AnsiConsole.MarkupLine($"[red]{Markup.Escape(msg)}[/]");
            Pause();
            return false;
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything | ExceptionFormats.ShowLinks);
            Pause();
            return false;
        }
    }

    public static void WriteFriendlyDbUpdateException(DbUpdateException ex)
    {
        var combined = $"{ex.Message} {ex.InnerException?.Message}".ToLowerInvariant();

        if (combined.Contains("foreign key", StringComparison.Ordinal)
            || combined.Contains("cannot delete or update", StringComparison.Ordinal)
            || combined.Contains("violates foreign key", StringComparison.Ordinal)
            || combined.Contains("clave foránea", StringComparison.Ordinal))
        {
            AnsiConsole.MarkupLine(
                "[red]No se pudo guardar: hay un conflicto con datos relacionados (clave foránea). Revise los identificadores e inténtelo de nuevo.[/]");
            return;
        }

        if (combined.Contains("duplicate", StringComparison.Ordinal)
            || combined.Contains("unique", StringComparison.Ordinal)
            || combined.Contains("uniq_", StringComparison.Ordinal)
            || combined.Contains("duplicate entry", StringComparison.Ordinal)
            || combined.Contains("índice único", StringComparison.Ordinal))
        {
            AnsiConsole.MarkupLine(
                "[red]No se pudo guardar: ya existe un registro con esos datos (duplicado o restricción única).[/]");
            return;
        }

        AnsiConsole.MarkupLine($"[red]{Markup.Escape(TruncateMessage(ex.Message))}[/]");
    }

    private static string TruncateMessage(string message)
    {
        if (string.IsNullOrEmpty(message))
            return message;
        return message.Length <= VarcharSafeMax ? message : message[..VarcharSafeMax] + "…";
    }

    private static void Pause()
    {
        AnsiConsole.MarkupLine("[grey]Presiona una tecla para continuar...[/]");
        Console.ReadKey(intercept: true);
    }
}
