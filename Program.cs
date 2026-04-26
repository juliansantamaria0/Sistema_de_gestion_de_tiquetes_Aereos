using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Infrastructure;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;


var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddSharedInfrastructure(builder.Configuration);
builder.Services.AddSingleton<ReportsMenu>();
builder.Services.AddSingleton<MainMenu>();

using var host = builder.Build();

await using (var scope = host.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await BootstrapDataSeeder.SeedAsync(db);

    var menu = scope.ServiceProvider.GetRequiredService<MainMenu>();
    await menu.RunAsync();
}

internal sealed class MainMenu(
    IEnumerable<IModuleUI> modules,
    ReportsMenu reportsMenu,
    IConfiguration configuration,
    AppDbContext dbContext,
    IServiceProvider serviceProvider)
{
    private readonly Dictionary<string, IModuleUI> _modules = BuildNormalizedModuleMap(modules);
    private readonly ReportsMenu _reportsMenu = reportsMenu;
    private readonly string _adminPin = configuration["AdminPortal:Pin"] ?? "0000";
    private readonly AuthService _authService = new(dbContext);
    private readonly AppDbContext _context = dbContext;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    private static Dictionary<string, IModuleUI> BuildNormalizedModuleMap(IEnumerable<IModuleUI> modules)
    {
        var dict = new Dictionary<string, IModuleUI>(StringComparer.Ordinal);
        foreach (var m in modules)
        {
            var nk = ModuleKeyNormalizer.Normalize(m.Key);
            if (string.IsNullOrEmpty(nk))
                continue;
            if (!dict.ContainsKey(nk))
                dict[nk] = m;
        }

        return dict;
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            RenderAccessPortal();

            var accessChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]¿A dónde desea ir?[/]")
                    .PageSize(6)
                    .AddChoices(
                    [
                        PortalAccess.AdminPortalLabel,
                        PortalAccess.ClientPortalLabel,
                        "Salir"
                    ]));

            if (accessChoice == "Salir")
                break;

            if (accessChoice == PortalAccess.AdminPortalLabel)
                await ShowAdminMenuAsync(cancellationToken);
            else
                await ShowUserMenuAsync(cancellationToken);
        }

        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[green]Hasta luego.[/]");
    }

    private static void RenderAccessPortal()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(
            new FigletText("Air Tickets")
                .Centered()
                .Color(Color.CornflowerBlue));

        AnsiConsole.MarkupLine("[grey]Sistema de gestión de tiquetes aéreos[/]");
        AnsiConsole.Write(new Rule("[green]Inicio[/]"));
        AnsiConsole.MarkupLine("[silver]Seleccione su portal de acceso.[/]");
        AnsiConsole.WriteLine();
    }

    private async Task ShowAdminMenuAsync(CancellationToken cancellationToken)
    {
        if (!TryVerifyAdminPin(cancellationToken))
            return;

        while (!cancellationToken.IsCancellationRequested)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[green] Administración [/]").RuleStyle(Style.Parse("grey")));
            AnsiConsole.MarkupLine("[grey]Gestión interna: vuelos, flota, tarifas, personal y configuración.[/]");
            AnsiConsole.WriteLine();

            var rootChoices = PortalAccess.AdminSections
                .Select(s => s.Title)
                .Append(PortalAccess.ReportsEntryLabel)
                .Append(PortalAccess.BackToAccessMenu)
                .ToList();

            var rootChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]¿A qué área desea acceder?[/]")
                    .PageSize(12)
                    .MoreChoicesText("[grey](Desplace para ver más)[/]")
                    .AddChoices(rootChoices));

            if (rootChoice == PortalAccess.BackToAccessMenu)
                return;

            if (rootChoice == PortalAccess.ReportsEntryLabel)
            {
                await ConsoleErrorHandler.RunSafeAsync(ct => _reportsMenu.RunAsync(ct), cancellationToken);
                continue;
            }

            var section = PortalAccess.AdminSections.First(s => s.Title == rootChoice);
            await RunAdminSectionAsync(section, cancellationToken);
        }
    }

    private async Task RunAdminSectionAsync(PortalAdminSection section, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule($"[green] Administración  ›  {Markup.Escape(section.Title)} [/]").RuleStyle(Style.Parse("grey")));
            AnsiConsole.MarkupLine($"[grey]{Markup.Escape(section.Description)}[/]");
            AnsiConsole.WriteLine();

            var items = BuildOrderedModuleItems(section.ModuleKeys);

            if (items.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No hay módulos registrados en esta área.[/]");
                PauseReturn();
                return;
            }

            var selected = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]¿Qué módulo desea gestionar?[/]")
                    .PageSize(18)
                    .MoreChoicesText("[grey](Desplace para ver todos los módulos)[/]")
                    .AddChoices(items.Select(x => x.Title).Append(PortalAccess.BackToAdminRoot)));

            if (selected == PortalAccess.BackToAdminRoot)
                return;

            var item   = items.First(x => x.Title == selected);
            var module = _modules[item.NormalizedKey];
            await RunModuleUiSafeAsync(module, $"Administración > {section.Title}", item.Title, cancellationToken);
        }
    }

    private async Task ShowUserMenuAsync(CancellationToken cancellationToken)
    {
        if (!CurrentUser.IsAuthenticated)
        {
            var authenticated = await ShowClientLoginAsync(cancellationToken);
            if (!authenticated)
                return;
        }

        while (!cancellationToken.IsCancellationRequested)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule($"[green] Portal de clientes [/]").RuleStyle(Style.Parse("grey")));
            AnsiConsole.MarkupLine($"[grey]Bienvenido, [bold]{Markup.Escape(CurrentUser.Username ?? "usuario")}[/]. Seleccione qué desea hacer.[/]");
            AnsiConsole.WriteLine();

            var mainOptions = new List<string> { "Reservar un vuelo" }
                .Concat(PortalAccess.ClientSections.Select(s => s.Title))
                .Append("─────────────────")
                .Append("Cerrar sesión")
                .ToList();

            var sectionTitle = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]¿Qué desea hacer?[/]")
                    .PageSize(12)
                    .AddChoices(mainOptions));

            if (sectionTitle == "Cerrar sesión")
            {
                AuthService.Logout();
                AnsiConsole.Clear();
                AnsiConsole.MarkupLine("[green]Sesión cerrada. Hasta pronto.[/]");
                PauseReturn();
                return;
            }

            if (sectionTitle == "─────────────────")
                continue;

            if (sectionTitle == "Reservar un vuelo")
            {
                var reservationService = _serviceProvider.GetRequiredService<IReservationService>();
                var wizard = new Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.UI.ReservationBookingWizardUI(
                    reservationService,
                    _context);
                await ConsoleErrorHandler.RunSafeAsync(ct => wizard.RunAsync(ct), cancellationToken);
                continue;
            }

            var section = PortalAccess.ClientSections.First(s => s.Title == sectionTitle);
            await RunClientSectionAsync(section, cancellationToken);
        }
    }

    private async Task<bool> ShowClientLoginAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[green] Portal de clientes — Acceso [/]").RuleStyle(Style.Parse("grey")));
            AnsiConsole.MarkupLine("[grey]Inicie sesión con su cuenta o regístrese si es la primera vez.[/]");
            AnsiConsole.WriteLine();

            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]¿Cómo desea continuar?[/]")
                    .PageSize(4)
                    .AddChoices("Iniciar sesión", "Crear cuenta nueva", PortalAccess.BackToAccessMenu));

            if (option == PortalAccess.BackToAccessMenu)
                return false;

            if (option == "Iniciar sesión")
            {
                var loginResult = await ShowLoginFormAsync(cancellationToken);
                if (loginResult) return true;
            }
            else if (option == "Crear cuenta nueva")
            {
                var registerResult = await ShowRegisterFormAsync(cancellationToken);
                if (registerResult) return true;
            }
        }
        return false;
    }

    private async Task<bool> ShowLoginFormAsync(CancellationToken cancellationToken)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[green] Iniciar sesión [/]").RuleStyle(Style.Parse("grey")));
        AnsiConsole.MarkupLine("[grey]Ingrese su usuario y contraseña.[/]");
        AnsiConsole.WriteLine();

        var username = AnsiConsole.Prompt(
            new TextPrompt<string>("[yellow]Usuario:[/]")
                .Validate(s => !string.IsNullOrWhiteSpace(s), "El usuario no puede estar vacío."));

        var password = AnsiConsole.Prompt(
            new TextPrompt<string>("[yellow]Contraseña:[/]")
                .Secret());

        AnsiConsole.WriteLine();

        var result = await _authService.LoginAsync(username, password, cancellationToken);

        if (!result.IsSuccess)
        {
            AnsiConsole.MarkupLine($"[red]  {Markup.Escape(result.ErrorMessage ?? "Credenciales incorrectas.")}[/]");
            PauseReturn();
            return false;
        }

        AnsiConsole.MarkupLine($"[green]  Bienvenido, {Markup.Escape(result.Username ?? username)}![/]");
        PauseReturn();
        return true;
    }

    private async Task<bool> ShowRegisterFormAsync(CancellationToken cancellationToken)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[green] Crear cuenta nueva [/]").RuleStyle(Style.Parse("grey")));
        AnsiConsole.MarkupLine("[grey]Complete el formulario para registrarse. Todos los campos marcados son obligatorios.[/]");
        AnsiConsole.WriteLine();

        var username = AnsiConsole.Prompt(
            new TextPrompt<string>("[yellow]Usuario:[/]")
                .Validate(s => !string.IsNullOrWhiteSpace(s), "El usuario no puede estar vacío."));

        var password = AnsiConsole.Prompt(
            new TextPrompt<string>("[yellow]Contraseña:[/]")
                .Secret()
                .Validate(s => s.Length >= 4, "La contraseña debe tener al menos 4 caracteres."));

        var confirmPassword = AnsiConsole.Prompt(
            new TextPrompt<string>("[yellow]Confirmar contraseña:[/]")
                .Secret());

        if (password != confirmPassword)
        {
            AnsiConsole.MarkupLine("[red]Las contraseñas no coinciden.[/]");
            PauseReturn();
            return false;
        }

        var firstName = AnsiConsole.Prompt(
            new TextPrompt<string>("[yellow]Nombre:[/]")
                .Validate(s => !string.IsNullOrWhiteSpace(s), "El nombre es obligatorio."));

        var lastName = AnsiConsole.Prompt(
            new TextPrompt<string>("[yellow]Apellido:[/]")
                .Validate(s => !string.IsNullOrWhiteSpace(s), "El apellido es obligatorio."));

        // Obtener tipos de documento disponibles
        var docTypes = await _context.DocumentTypes.AsNoTracking().ToListAsync(cancellationToken);
        if (!docTypes.Any())
        {
            AnsiConsole.MarkupLine("[red]No hay tipos de documento disponibles.[/]");
            PauseReturn();
            return false;
        }

        var docTypeChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]Tipo de documento:[/]")
                .AddChoices(docTypes.Select(d => $"{d.DocumentTypeId} - {d.Name}")));

        var documentTypeId = int.Parse(docTypeChoice.Split(new[] { " - " }, StringSplitOptions.None)[0]);

        var documentNumber = AnsiConsole.Prompt(
            new TextPrompt<string>("[yellow]Número de documento:[/]")
                .Validate(s => !string.IsNullOrWhiteSpace(s), "El número de documento es obligatorio."));

        var email = AnsiConsole.Prompt(
            new TextPrompt<string>("[yellow]Email (opcional):[/]")
                .AllowEmpty()
                .Validate(s =>
                {
                    if (string.IsNullOrWhiteSpace(s)) return ValidationResult.Success();
                    return s.Contains('@') && s.LastIndexOf('.') > s.IndexOf('@') + 1
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Ingresa un email válido (ej: usuario@dominio.com)[/]");
                }));

        var phone = AnsiConsole.Prompt(
            new TextPrompt<string>("[yellow]Teléfono (opcional):[/]"));

        var result = await _authService.RegisterAsync(
            username, password, firstName, lastName,
            documentTypeId, documentNumber,
            string.IsNullOrWhiteSpace(email) ? null : email,
            string.IsNullOrWhiteSpace(phone) ? null : phone,
            cancellationToken);

        if (!result.IsSuccess)
        {
            AnsiConsole.MarkupLine($"[red]{result.ErrorMessage}[/]");
            PauseReturn();
            return false;
        }

        AnsiConsole.MarkupLine($"[green]Cuenta creada exitosamente! Bienvenido, {result.Username}![/]");
        PauseReturn();
        return true;
    }

    private async Task RunClientSectionAsync(PortalClientSection section, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule($"[green] {Markup.Escape(section.Title)} [/]").RuleStyle(Style.Parse("grey")));
            AnsiConsole.MarkupLine($"[grey]{Markup.Escape(section.Description)}[/]");
            AnsiConsole.WriteLine();

            var items = BuildOrderedModuleItems(section.ModuleKeys);

            if (items.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No hay módulos disponibles en esta sección.[/]");
                PauseReturn();
                return;
            }

            var selected = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]¿Qué desea consultar o gestionar?[/]")
                    .PageSize(15)
                    .MoreChoicesText("[grey](Desplace para ver más)[/]")
                    .AddChoices(items.Select(x => x.Title).Append(PortalAccess.BackToClientAreas)));

            if (selected == PortalAccess.BackToClientAreas)
                return;

            var item   = items.First(x => x.Title == selected);
            var module = _modules[item.NormalizedKey];
            await RunModuleUiSafeAsync(module, section.Title, item.Title, cancellationToken);
        }
    }

    private async Task RunModuleUiSafeAsync(IModuleUI module, string portalContext, string moduleTitle, CancellationToken cancellationToken)
    {
        AnsiConsole.Clear();
        await ConsoleErrorHandler.RunSafeAsync(ct => module.RunAsync(ct), cancellationToken);
    }

    private bool TryVerifyAdminPin(CancellationToken cancellationToken)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[green] Administración — Autenticación [/]").RuleStyle(Style.Parse("grey")));
        AnsiConsole.MarkupLine("[grey]Ingrese el PIN de administrador. Deje vacío y presione Enter para volver.[/]");
        AnsiConsole.WriteLine();

        while (!cancellationToken.IsCancellationRequested)
        {
            var pinRaw = AnsiConsole.Prompt(
                new TextPrompt<string>("[yellow]PIN de administrador:[/] [grey](Enter vacío = volver)[/]")
                    .Secret()
                    .AllowEmpty());

            if (string.IsNullOrEmpty(pinRaw))
                return false;

            var pin = pinRaw.Length > 60 ? pinRaw[..60] : pinRaw;

            if (string.Equals(pin, _adminPin, StringComparison.Ordinal))
                return true;

            AnsiConsole.MarkupLine("[red]PIN incorrecto. Intente de nuevo.[/]");
        }

        return false;
    }

    private List<PortalMenuItem> BuildOrderedModuleItems(IReadOnlyList<string> moduleKeys) =>
        moduleKeys
            .Select(k => ModuleKeyNormalizer.Normalize(k))
            .Where(k => _modules.ContainsKey(k))
            .Select(k => new PortalMenuItem(k, FunctionalNavigation.GetModuleTitle(k)))
            .ToList();

    private static void PauseReturn()
    {
        AnsiConsole.MarkupLine("[grey]Presiona una tecla para continuar...[/]");
        Console.ReadKey(intercept: true);
    }

    private sealed record PortalMenuItem(string NormalizedKey, string Title);
}

internal sealed class ReportsMenu(IServiceScopeFactory scopeFactory)
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[green] Administración  ›  Reportes [/]").RuleStyle(Style.Parse("grey")));
            AnsiConsole.MarkupLine("[grey]Resúmenes y estadísticas del sistema en tiempo real.[/]");
            AnsiConsole.WriteLine();

            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]¿Qué reporte desea consultar?[/]")
                    .PageSize(12)
                    .AddChoices(
                    [
                        "Reservas por estado",
                        "Vuelos con más reservas",
                        "Clientes con más reservas",
                        "Tiquetes emitidos por estado",
                        "Ingresos por estado de pago",
                        "Disponibilidad de asientos por vuelo",
                        "« Volver"
                    ]));

            if (option == "« Volver")
                return;

            var ok = await ConsoleErrorHandler.TryRunSafeAsync(async ct =>
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                switch (option)
                {
                    case "Reservas por estado":
                        await RenderReservationsByStatusAsync(db, ct);
                        break;
                    case "Vuelos con más reservas":
                        await RenderFlightsWithMostReservationsAsync(db, ct);
                        break;
                    case "Clientes con más reservas":
                        await RenderTopCustomersAsync(db, ct);
                        break;
                    case "Tiquetes emitidos por estado":
                        await RenderTicketsByStatusAsync(db, ct);
                        break;
                    case "Ingresos por estado de pago":
                        await RenderRevenueByPaymentStatusAsync(db, ct);
                        break;
                    case "Disponibilidad de asientos por vuelo":
                        await RenderSeatAvailabilityAsync(db, ct);
                        break;
                }
            }, cancellationToken);

            if (ok)
            {
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine("[grey]Presiona una tecla para continuar...[/]");
                Console.ReadKey(intercept: true);
            }
        }
    }

    private static async Task RenderReservationsByStatusAsync(AppDbContext db, CancellationToken ct)
    {
        var data = await db.Reservations
            .AsNoTracking()
            .Join(db.ReservationStatuses.AsNoTracking(),
                reservation => reservation.ReservationStatusId,
                status => status.Id,
                (reservation, status) => new { status.Name })
            .GroupBy(x => x.Name)
            .Select(g => new { Estado = g.Key, Cantidad = g.Count() })
            .OrderByDescending(x => x.Cantidad)
            .ToListAsync(ct);

        RenderSimpleTable("Reservas por estado", "Estado", "Cantidad", data.Select(x => (x.Estado, x.Cantidad.ToString())));
    }

    private static async Task RenderFlightsWithMostReservationsAsync(AppDbContext db, CancellationToken ct)
    {
        var data = await db.Reservations
            .AsNoTracking()
            .Join(db.ScheduledFlights.AsNoTracking(),
                reservation => reservation.ScheduledFlightId,
                scheduled => scheduled.Id,
                (reservation, scheduled) => new { reservation, scheduled })
            .Join(db.BaseFlights.AsNoTracking(),
                rs => rs.scheduled.BaseFlightId,
                baseFlight => baseFlight.Id,
                (rs, baseFlight) => new
                {
                    baseFlight.FlightCode,
                    rs.scheduled.DepartureDate,
                    rs.reservation.Id
                })
            .GroupBy(x => new { x.FlightCode, x.DepartureDate })
            .Select(g => new
            {
                Vuelo = g.Key.FlightCode,
                Fecha = g.Key.DepartureDate,
                Reservas = g.Count()
            })
            .OrderByDescending(x => x.Reservas)
            .ThenBy(x => x.Vuelo)
            .Take(10)
            .ToListAsync(ct);

        var table = new Table().Border(TableBorder.Rounded).Expand();
        table.AddColumn("Vuelo");
        table.AddColumn("Fecha");
        table.AddColumn("Reservas");
        foreach (var row in data)
            table.AddRow(row.Vuelo, row.Fecha.ToString("yyyy-MM-dd"), row.Reservas.ToString());
        AnsiConsole.Write(new Panel(table).Header("[green]Vuelos con más reservas[/]"));
    }

    private static async Task RenderTopCustomersAsync(AppDbContext db, CancellationToken ct)
    {
        var data = await db.Reservations
            .AsNoTracking()
            .GroupBy(r => r.CustomerId)
            
            .Select(g => new { Cliente = g.Key, Reservas = g.Count() })
            .OrderByDescending(x => x.Reservas)
            .Take(10)
            .ToListAsync(ct);

        RenderSimpleTable("Clientes con más reservas", "Cliente ID", "Reservas", data.Select(x => (x.Cliente.ToString(), x.Reservas.ToString())));
    }

    private static async Task RenderTicketsByStatusAsync(AppDbContext db, CancellationToken ct)
    {
        var data = await db.Tickets
            .AsNoTracking()
            .Join(db.TicketStatuses.AsNoTracking(),
                ticket => ticket.TicketStatusId,
                status => status.Id,
                (ticket, status) => new { status.Name })
            .GroupBy(x => x.Name)
            .Select(g => new { Estado = g.Key, Cantidad = g.Count() })
            .OrderByDescending(x => x.Cantidad)
            .ToListAsync(ct);

        RenderSimpleTable("Tiquetes emitidos por estado", "Estado", "Cantidad", data.Select(x => (x.Estado, x.Cantidad.ToString())));
    }

    private static async Task RenderRevenueByPaymentStatusAsync(AppDbContext db, CancellationToken ct)
    {
        var data = await db.Payments
            .AsNoTracking()
            .Join(db.PaymentStatuses.AsNoTracking(),
                payment => payment.PaymentStatusId,
                status => status.Id,
                (payment, status) => new { payment.Amount, status.Name })
            .GroupBy(x => x.Name)
            .Select(g => new { Estado = g.Key, Total = g.Sum(x => x.Amount) })
            .OrderByDescending(x => x.Total)
            .ToListAsync(ct);

        var table = new Table().Border(TableBorder.Rounded).Expand();
        table.AddColumn("Estado de pago");
        table.AddColumn("Monto total");
        foreach (var row in data)
            table.AddRow(row.Estado, row.Total.ToString("N2"));
        AnsiConsole.Write(new Panel(table).Header("[green]Ingresos por estado de pago[/]"));
    }

    private static async Task RenderSeatAvailabilityAsync(AppDbContext db, CancellationToken ct)
    {
        var data = await db.FlightSeats
            .AsNoTracking()
            .Join(db.SeatStatuses.AsNoTracking(),
                seat => seat.SeatStatusId,
                status => status.Id,
                (seat, status) => new { seat.ScheduledFlightId, SeatStatus = status.Name })
            .GroupBy(x => new { x.ScheduledFlightId, x.SeatStatus })
            .Select(g => new { VueloProgramado = g.Key.ScheduledFlightId, EstadoAsiento = g.Key.SeatStatus, Cantidad = g.Count() })
            .OrderBy(x => x.VueloProgramado)
            .ThenBy(x => x.EstadoAsiento)
            .Take(50)
            .ToListAsync(ct);

        var table = new Table().Border(TableBorder.Rounded).Expand();
        table.AddColumn("Vuelo programado ID");
        table.AddColumn("Estado asiento");
        table.AddColumn("Cantidad");
        foreach (var row in data)
            table.AddRow(row.VueloProgramado.ToString(), row.EstadoAsiento, row.Cantidad.ToString());
        AnsiConsole.Write(new Panel(table).Header("[green]Disponibilidad de asientos por vuelo[/]"));
    }

    private static void RenderSimpleTable(string title, string leftColumn, string rightColumn, IEnumerable<(string Left, string Right)> rows)
    {
        var table = new Table().Border(TableBorder.Rounded).Expand();
        table.AddColumn(leftColumn);
        table.AddColumn(rightColumn);
        foreach (var row in rows)
            table.AddRow(Markup.Escape(row.Left), Markup.Escape(row.Right));

        AnsiConsole.Write(new Panel(table).Header($"[green]{Markup.Escape(title)}[/]"));
    }
}

internal static class FunctionalNavigation
{
    private static readonly Dictionary<string, string> ModuleTitles = new(StringComparer.OrdinalIgnoreCase)
    {
        ["airline"] = "Aerolíneas",
        ["country"] = "Países",
        ["city"] = "Ciudades y destinos",
        ["airport"] = "Aeropuertos",
        ["terminal"] = "Terminales",
        ["gate"] = "Puertas de embarque",
        ["route"] = "Rutas",
        ["baseflight"] = "Vuelos base",
        ["routeschedule"] = "Horarios de ruta",
        ["scheduledflight"] = "Vuelos programados",
        ["flightstatus"] = "Estados de vuelo",
        ["flightstatushistory"] = "Historial de estados de vuelo",
        ["aircraftmanufacturer"] = "Fabricantes de aeronaves",
        ["aircrafttype"] = "Tipos de aeronave",
        ["aircraft"] = "Aeronaves",
        ["cabinclass"] = "Clases de cabina",
        ["seatstatus"] = "Estados de asiento",
        ["seatmap"] = "Mapas de asiento",
        ["flightseat"] = "Asientos por vuelo",
        ["flightcabinprice"] = "Precios por cabina",
        ["customer"] = "Clientes",
        ["person"] = "Personas",
        ["passenger"] = "Pasajeros",
        ["documenttype"] = "Tipos de documento",
        ["gender"] = "Géneros",
        ["nationality"] = "Nacionalidades",
        ["contacttype"] = "Tipos de contacto",
        ["passengercontact"] = "Contactos de pasajero",
        ["reservation"] = "Reservas",
        ["reservationdetail"] = "Detalles de reserva",
        ["reservationstatus"] = "Estados de reserva",
        ["reservationstatushistory"] = "Trazabilidad de reservas",
        ["ticket"] = "Tiquetes",
        ["ticketstatus"] = "Estados de tiquete",
        ["ticketstatushistory"] = "Trazabilidad de tiquetes",
        ["ticketbaggage"] = "Equipaje por tiquete",
        ["baggageallowance"] = "Políticas de equipaje",
        ["baggagetype"] = "Tipos de equipaje",
        ["checkin"] = "Check-in",
        ["checkinstatus"] = "Estados de check-in",
        ["boardingpass"] = "Boarding passes",
        ["payment"] = "Pagos",
        ["paymentstatus"] = "Estados de pago",
        ["paymentmethod"] = "Métodos de pago",
        ["currency"] = "Monedas",
        ["refund"] = "Reembolsos",
        ["refundstatus"] = "Estados de reembolso",
        ["promotion"] = "Promociones",
        ["flightpromotion"] = "Promociones por vuelo",
        ["faretype"] = "Tipos de tarifa",
        ["discounttype"] = "Tipos de descuento",
        ["passengerdiscount"] = "Descuentos de pasajero",
        ["crewrole"] = "Roles de tripulación",
        ["jobposition"] = "Cargos",
        ["employee"] = "Empleados",
        ["flightcrew"] = "Tripulación por vuelo",
        ["delayreason"] = "Motivos de retraso",
        ["flightdelay"] = "Retrasos de vuelo",
        ["cancellationreason"] = "Motivos de cancelación",
        ["flightcancellation"] = "Cancelaciones de vuelo",
        ["loyaltyprogram"] = "Programas de lealtad",
        ["loyaltytier"] = "Niveles de lealtad",
        ["loyaltyaccount"] = "Cuentas de lealtad",
        ["loyaltytransaction"] = "Movimientos de lealtad",
        ["role"] = "Roles",
        ["permission"] = "Permisos",
        ["rolepermission"] = "Permisos por rol",
        ["user"] = "Usuarios",
    };

    public static string GetModuleTitle(string normalizedKey)
        => ModuleTitles.TryGetValue(normalizedKey, out var title) ? title : normalizedKey;

    public static IReadOnlyList<string> GetModuleKeysWithTitlePrefix(string prefix) =>
        ModuleTitles
            .Where(kv => kv.Value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            .Select(kv => kv.Key)
            .Distinct(StringComparer.Ordinal)
            .ToArray();
}

internal static class PortalAccess
{
    public const string AdminPortalLabel = "[[1]] Administración";
    public const string ClientPortalLabel = "[[2]] Portal de clientes";
    public const string BackToAccessMenu = "« Volver";
    public const string BackToAdminRoot = "« Volver";
    public const string BackToClientAreas = "« Volver";
    public const string ReportsEntryLabel = "Reportes";

    public static readonly IReadOnlyList<PortalAdminSection> AdminSections =
    [
        new PortalAdminSection(
            "Vuelos y Flota",
            "Vuelos programados, rutas, aeronaves, aeropuertos, terminales, puertas y tripulación.",
            ["scheduledflight", "baseflight", "routeschedule", "route", "airline", "aircraft", "aircraftmanufacturer", "aircrafttype", "airport", "terminal", "gate", "flightstatushistory", "delayreason", "flightdelay", "cancellationreason", "flightcancellation"]),
        new PortalAdminSection(
            "Comercial y Tarifas",
            "Precios, asientos, mapas, promociones, equipaje, tipos de tarifa y descuentos.",
            ["flightcabinprice", "flightseat", "seatmap", "cabinclass", "promotion", "flightpromotion", "baggageallowance", "baggagetype", "faretype", "discounttype"]),
        new PortalAdminSection(
            "Seguridad y Personal",
            "Usuarios, roles, permisos, empleados y tripulación de vuelo.",
            ["user", "role", "permission", "rolepermission", "employee", "jobposition", "crewrole", "flightcrew"]),
        new PortalAdminSection(
            "Configuración",
            "Países, ciudades, monedas, documentos, géneros, contactos, programas de lealtad y métodos de pago.",
            ["currency", "paymentmethod", "documenttype", "contacttype", "gender", "nationality", "loyaltyprogram", "loyaltytier", "country", "city"]),
        new PortalAdminSection(
            "Catálogos de estados",
            "Estados de vuelo, reserva, tiquete, pago, check-in, reembolso y asiento.",
            FunctionalNavigation.GetModuleKeysWithTitlePrefix("Estados de "))
    ];

    public static readonly HashSet<string> ClientPortalNormalizedKeys =
    [
        "scheduledflight", "customer", "person", "passenger", "passengercontact",
        "reservation", "reservationdetail", "reservationstatushistory",
        "ticket", "ticketstatushistory", "ticketbaggage", "checkin", "boardingpass",
        "payment", "refund", "passengerdiscount", "loyaltyaccount", "loyaltytransaction"
    ];

    public static readonly IReadOnlyList<PortalClientSection> ClientSections =
    [
        new PortalClientSection(
            "Mis reservas",
            "Consulta y seguimiento de tus reservas activas.",
            ["reservation", "reservationstatushistory", "reservationdetail", "passenger", "passengercontact", "passengerdiscount", "customer", "person"]),
        new PortalClientSection(
            "Mis tiquetes y check-in",
            "Tiquetes emitidos, equipaje registrado, check-in y pases de abordar.",
            ["ticket", "checkin", "boardingpass", "ticketbaggage", "ticketstatushistory"]),
        new PortalClientSection(
            "Mis pagos y reembolsos",
            "Historial de pagos y solicitudes de reembolso.",
            ["payment", "refund"]),
        new PortalClientSection(
            "Vuelos disponibles",
            "Consulta la oferta de vuelos programados.",
            ["scheduledflight"]),
        new PortalClientSection(
            "Programa de lealtad",
            "Cuentas y movimientos de tus millas acumuladas.",
            ["loyaltyaccount", "loyaltytransaction"])
    ];
}

internal sealed record PortalClientSection(string Title, string Description, IReadOnlyList<string> ModuleKeys);

internal sealed record PortalAdminSection(string Title, string Description, IReadOnlyList<string> ModuleKeys);
