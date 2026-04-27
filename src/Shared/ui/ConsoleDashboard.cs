using Spectre.Console;
using Spectre.Console.Rendering;

namespace Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

/// <summary>Marco visual común: paneles, tablas y mensajes de feedback (estilo “dashboard” en consola).</summary>
public static class ConsoleDashboard
{
    public static void RenderAccessLanding()
    {
        AnsiConsole.Clear();
        AnsiConsole.WriteLine();
        AnsiConsole.Write(
            new FigletText("Air Tickets")
                .Centered()
                .Color(Color.SkyBlue1));

        var blurb = new Markup(
            "[bold silver]Sistema de gestión de tiquetes aéreos[/]\n" +
            "[grey]Elija su portal. Use las [bold]flechas[/] y [bold]Enter[/].[/]");

        AnsiConsole.Write(
            new Panel(blurb)
                .Header("[white on deepskyblue2]  INICIO  [/]")
                .Border(BoxBorder.Double)
                .BorderStyle(Style.Parse("grey"))
                .Padding(1, 0)
                .Expand());
        AnsiConsole.WriteLine();
        NavigationHint();
    }

    /// <summary>Texto para <see cref="SelectionPrompt{T}.MoreChoicesText"/> (listas largas).</summary>
    public const string SelectionMoreChoicesNav = "[grey]↑ ↓  Desplace si el menú es largo.[/]";

    /// <summary>Línea breve: cómo moverse con Spectre; suele usarse bajo títulos de portal/módulo.</summary>
    public static void NavigationHint() =>
        AnsiConsole.MarkupLine("[dim]⯈ Navegación: [bold]↑ ↓ Enter[/] · La opción [bold]« Volver[/] suele estar al final.[/]");

    public static void RenderClientPortalHeader(string subline)
    {
        AnsiConsole.Clear();
        AnsiConsole.WriteLine();
        var body = new Markup(
            "[bold aquamarine1]Bienvenido al espacio de pasajero[/]\n" +
            "[grey]" + Markup.Escape(subline) + "[/]");

        AnsiConsole.Write(
            new Panel(body)
                .Header("[black on seagreen1]  PORTAL DE CLIENTES  [/]")
                .Border(BoxBorder.Double)
                .BorderStyle(Style.Parse("grey"))
                .Padding(1, 1)
                .Expand());
        AnsiConsole.WriteLine();
    }

    public static void SubScreenTitle(string shortTitle, string? hint = null)
    {
        AnsiConsole.Write(
            new Rule($"[seagreen1 bold]{Markup.Escape(shortTitle)}[/]")
                .RuleStyle(Style.Parse("grey")));
        if (!string.IsNullOrWhiteSpace(hint))
            AnsiConsole.MarkupLine("[dim]⯈ " + Markup.Escape(hint) + "[/]");
        AnsiConsole.WriteLine();
    }

    public static void Step(int number, string title, string? detail = null)
    {
        var line = new Rule($"[aqua]Paso {number}[/]  [bold white]{Markup.Escape(title)}[/]")
            .RuleStyle(Style.Parse("grey"));
        AnsiConsole.Write(line);
        if (!string.IsNullOrWhiteSpace(detail))
            AnsiConsole.MarkupLine("[dim]  " + Markup.Escape(detail) + "[/]");
        AnsiConsole.WriteLine();
    }

    public static Table NewDataTable()
    {
        return new Table()
            .Border(TableBorder.Heavy)
            .BorderStyle(Style.Parse("grey"))
            .Expand();
    }

    public static void ShowTablePanel(string header, Table table)
    {
        AnsiConsole.Write(
            new Panel(table)
                .Header($"[seagreen1 bold]{Markup.Escape(header)}[/]")
                .Border(BoxBorder.Rounded)
                .BorderStyle(Style.Parse("grey"))
                .Expand());
    }

    /// <summary>Tablas de módulos CRUD, reportes y listados técnicos (tema “workspace”).</summary>
    public static void ShowWorkspaceTablePanel(string header, Table table)
    {
        AnsiConsole.Write(
            new Panel(table)
                .Header($"[bold steelblue1]{Markup.Escape(header)}[/]")
                .Border(BoxBorder.Rounded)
                .BorderStyle(Style.Parse("grey"))
                .Expand());
    }

    /// <summary>Detalle de un registro (grids reflectivos, etc.).</summary>
    public static void ShowWorkspaceDetailPanel(string header, IRenderable content) =>
        AnsiConsole.Write(
            new Panel(content)
                .Header($"[slateblue1 bold]{Markup.Escape(header)}[/]")
                .Border(BoxBorder.Rounded)
                .BorderStyle(Style.Parse("grey"))
                .Expand());

    /// <summary>Cabecera del área de administración (submenús, PIN, reportes).</summary>
    public static void AdminSubScreenTitle(string shortTitle, string? hint = null)
    {
        AnsiConsole.Write(
            new Rule($"[bold orange1]{Markup.Escape(shortTitle)}[/]")
                .RuleStyle(Style.Parse("grey")));
        if (!string.IsNullOrWhiteSpace(hint))
            AnsiConsole.MarkupLine("[dim]⯈ " + Markup.Escape(hint) + "[/]");
        AnsiConsole.WriteLine();
    }

    public static void RenderAdminBackOfficeHeader(string subline)
    {
        AnsiConsole.Clear();
        AnsiConsole.WriteLine();
        var body = new Markup(
            "[bold darkorange]Operaciones, catálogos y reportes internos[/]\n" +
            "[grey]" + Markup.Escape(subline) + "[/]");

        AnsiConsole.Write(
            new Panel(body)
                .Header("[black on orange1]  ADMINISTRACIÓN  [/]")
                .Border(BoxBorder.Double)
                .BorderStyle(Style.Parse("grey"))
                .Padding(1, 1)
                .Expand());
        AnsiConsole.WriteLine();
    }

    /// <summary>Módulos clásicos (cliente o admin): listas CRUD y acciones.</summary>
    public static void ModuleScreenHeader(string title, string? hint)
    {
        AnsiConsole.Write(
            new Rule($"[bold steelblue1]{Markup.Escape(title)}[/]")
                .RuleStyle(Style.Parse("grey")));
        if (!string.IsNullOrWhiteSpace(hint))
            AnsiConsole.MarkupLine(hint);
        AnsiConsole.WriteLine();
    }

    public static void Success(string message) =>
        AnsiConsole.Write(
            new Panel(
                    new Markup("[bold green]Hecho.[/] " + Markup.Escape(message)))
                .Border(BoxBorder.Rounded)
                .BorderStyle(Style.Parse("green"))
                .Padding(0, 0));

    public static void Warning(string message) =>
        AnsiConsole.Write(
            new Panel(new Markup("[bold yellow]Atención.[/] " + Markup.Escape(message)))
                .Border(BoxBorder.Rounded)
                .BorderStyle(Style.Parse("gold3"))
                .Padding(0, 0));

    public static void Error(string message) =>
        AnsiConsole.Write(
            new Panel(new Markup("[bold red]Error.[/] " + Markup.Escape(message)))
                .Border(BoxBorder.Rounded)
                .BorderStyle(Style.Parse("red"))
                .Padding(0, 0));

    public static void Info(string message) =>
        AnsiConsole.MarkupLine($"[grey]  ℹ  [italic]{Markup.Escape(message)}[/][/]");

    public static void FooterPressKey() =>
        AnsiConsole.MarkupLine("\n[grey]── Presione una tecla para continuar ──[/]");

    public static void MiniDivider() =>
        AnsiConsole.Write(new Rule("").RuleStyle(Style.Parse("grey")));
}
