using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console;
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
    AppDbContext dbContext)
{
    private readonly Dictionary<string, IModuleUI> _modules = BuildNormalizedModuleMap(modules);
    private readonly ReportsMenu _reportsMenu = reportsMenu;
    private readonly string _adminPin = configuration["AdminPortal:Pin"] ?? "0000";
    private readonly AuthService _authService = new(dbContext);
    private readonly AppDbContext _context = dbContext;

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
                    .Title("[yellow]Menú de acceso[/]")
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
        AnsiConsole.Write(new Rule("[green]Menú de acceso[/]"));
        AnsiConsole.MarkupLine(
            "[silver]Elija el portal: administración (catálogos y operación interna) o clientes (reservas, tiquetes y pagos).[/]");
        AnsiConsole.WriteLine();
    }

    private async Task ShowAdminMenuAsync(CancellationToken cancellationToken)
    {
        if (!TryVerifyAdminPin(cancellationToken))
            return;

        while (!cancellationToken.IsCancellationRequested)
        {
            AnsiConsole.Clear();
            RenderPortalBreadcrumb("Portal administrativo", "Catálogos, configuración y operación interna.");

            var rootChoices = PortalAccess.AdminSections
                .Select(s => s.Title)
                .Append(PortalAccess.ReportsEntryLabel)
                .Append(PortalAccess.BackToAccessMenu)
                .ToList();

            var rootChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Seleccione un área[/]")
                    .PageSize(12)
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
            RenderPortalBreadcrumb($"Portal administrativo > {section.Title}", section.Description);

            var items = BuildOrderedModuleItems(section.ModuleKeys);

            if (items.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No hay módulos registrados en esta área.[/]");
                PauseReturn();
                return;
            }

            var selected = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[yellow]{Markup.Escape(section.Title)}[/]")
                    .PageSize(18)
                    .AddChoices(items.Select(x => x.Title).Append(PortalAccess.BackToAdminRoot)));

            if (selected == PortalAccess.BackToAdminRoot)
                return;

            var item = items.First(x => x.Title == selected);
            var module = _modules[item.NormalizedKey];
            await RunModuleUiSafeAsync(module, $"Portal administrativo > {section.Title}", item.Title, cancellationToken);
        }
    }

    private async Task ShowUserMenuAsync(CancellationToken cancellationToken)
    {
        // Verificar si ya hay un usuario logueado
        if (!CurrentUser.IsAuthenticated)
        {
            var authenticated = await ShowClientLoginAsync(cancellationToken);
            if (!authenticated)
                return;
        }

        while (!cancellationToken.IsCancellationRequested)
        {
            AnsiConsole.Clear();
            RenderPortalBreadcrumb(
                $"Portal de clientes — [{CurrentUser.Username}]",
                "Flujos operativos: vuelos, reservas, tiquetes y pagos.");

            var sectionTitle = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Seleccione un área[/]")
                    .PageSize(10)
                    .AddChoices(PortalAccess.ClientSections.Select(s => s.Title))
                    .AddChoices("Cerrar sesión"));

            if (sectionTitle == "Cerrar sesión")
            {
                AuthService.Logout();
                AnsiConsole.MarkupLine("[green]Sesión cerrada correctamente.[/]");
                PauseReturn();
                return;
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
            RenderPortalBreadcrumb("Portal de clientes — Autenticación", "Ingrese o cree su cuenta.");

            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]¿Qué desea hacer?[/]")
                    .PageSize(3)
                    .AddChoices("Iniciar sesión", "Registrarse", PortalAccess.BackToAccessMenu));

            if (option == PortalAccess.BackToAccessMenu)
                return false;

            if (option == "Iniciar sesión")
            {
                var loginResult = await ShowLoginFormAsync(cancellationToken);
                if (loginResult)
                    return true;
            }
            else if (option == "Registrarse")
            {
                var registerResult = await ShowRegisterFormAsync(cancellationToken);
                if (registerResult)
                    return true;
            }
        }
        return false;
    }

    private async Task<bool> ShowLoginFormAsync(CancellationToken cancellationToken)
    {
        AnsiConsole.Clear();
        RenderPortalBreadcrumb("Portal de clientes — Iniciar sesión", "Ingrese sus credenciales.");

        var username = AnsiConsole.Prompt(
            new TextPrompt<string>("[yellow]Usuario:[/]")
                .Validate(s => !string.IsNullOrWhiteSpace(s), "El usuario es obligatorio."));

        var password = AnsiConsole.Prompt(
            new TextPrompt<string>("[yellow]Contraseña:[/]")
                .Secret());

        var result = await _authService.LoginAsync(username, password, cancellationToken);

        if (!result.IsSuccess)
        {
            AnsiConsole.MarkupLine($"[red]{result.ErrorMessage}[/]");
            PauseReturn();
            return false;
        }

        AnsiConsole.MarkupLine($"[green]Bienvenido, {result.Username}![/]");
        PauseReturn();
        return true;
    }

    private async Task<bool> ShowRegisterFormAsync(CancellationToken cancellationToken)
    {
        AnsiConsole.Clear();
        RenderPortalBreadcrumb("Portal de clientes — Registrarse", "Cree una nueva cuenta.");

        var username = AnsiConsole.Prompt(
            new TextPrompt<string>("[yellow]Usuario:[/]")
                .Validate(s => !string.IsNullOrWhiteSpace(s), "El usuario es obligatorio."));

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
            RenderPortalBreadcrumb($"Portal de clientes > {section.Title}", section.Description);

            var items = BuildOrderedModuleItems(section.ModuleKeys);

            if (items.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No hay módulos registrados en esta área.[/]");
                PauseReturn();
                return;
            }

            var selected = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[yellow]{Markup.Escape(section.Title)}[/]")
                    .PageSize(18)
                    .AddChoices(items.Select(x => x.Title).Append(PortalAccess.BackToClientAreas)));

            if (selected == PortalAccess.BackToClientAreas)
                return;

            var item = items.First(x => x.Title == selected);
            var module = _modules[item.NormalizedKey];
            await RunModuleUiSafeAsync(module, $"Portal de clientes > {section.Title}", item.Title, cancellationToken);
        }
    }

    private async Task RunModuleUiSafeAsync(IModuleUI module, string portalContext, string moduleTitle, CancellationToken cancellationToken)
    {
        AnsiConsole.Clear();
        RenderPortalBreadcrumb($"{portalContext} > {moduleTitle}", "Gestión del módulo");
        await ConsoleErrorHandler.RunSafeAsync(ct => module.RunAsync(ct), cancellationToken);
    }

    private bool TryVerifyAdminPin(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            AnsiConsole.Clear();
            RenderPortalBreadcrumb("Portal administrativo — autenticación", "Acceso restringido (demostración con PIN).");

            var step = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]¿Cómo desea continuar?[/]")
                    .AddChoices("Ingresar PIN", PortalAccess.BackToAccessMenu));

            if (step == PortalAccess.BackToAccessMenu)
                return false;

            var pinRaw = AnsiConsole.Prompt(
                new TextPrompt<string>("[yellow]PIN de acceso administrativo[/]")
                    .Secret());
            var pin = pinRaw.Length > 60 ? pinRaw[..60] : pinRaw;

            if (string.Equals(pin, _adminPin, StringComparison.Ordinal))
                return true;

            AnsiConsole.MarkupLine("[red]PIN incorrecto.[/]");
            PauseReturn();
        }

        return false;
    }

    private List<PortalMenuItem> BuildOrderedModuleItems(IReadOnlyList<string> moduleKeys) =>
        moduleKeys
            .Select(k => ModuleKeyNormalizer.Normalize(k))
            .Where(k => _modules.ContainsKey(k))
            .Select(k => new PortalMenuItem(k, FunctionalNavigation.GetModuleTitle(k)))
            .OrderBy(x => x.Title, StringComparer.CurrentCultureIgnoreCase)
            .ToList();

    private static void RenderPortalBreadcrumb(string breadcrumb, string description)
    {
        AnsiConsole.Write(new Rule($"[green]{Markup.Escape(breadcrumb)}[/]"));
        AnsiConsole.MarkupLine($"[grey]{Markup.Escape(description)}[/]");
        AnsiConsole.WriteLine();
    }

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
            AnsiConsole.Write(new Rule("[green]Portal administrativo > Reportes[/]"));
            AnsiConsole.MarkupLine("[grey]Reportes operativos generados con LINQ y datos persistidos en MySQL.[/]");
            AnsiConsole.WriteLine();

            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Seleccione un reporte[/]")
                    .PageSize(12)
                    .AddChoices(
                    [
                        "Reservas por estado",
                        "Vuelos con más reservas",
                        "Clientes con más reservas",
                        "Tiquetes emitidos por estado",
                        "Ingresos por estado de pago",
                        "Disponibilidad de asientos por vuelo",
                        "Volver"
                    ]));

            if (option == "Volver")
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
    public const string AdminPortalLabel = "[[1]] Portal administrativo";
    public const string ClientPortalLabel = "[[2]] Portal de clientes";
    public const string BackToAccessMenu = "Volver al menú de acceso";
    public const string BackToAdminRoot = "Volver al menú administrativo";
    public const string BackToClientAreas = "Volver a áreas del portal de clientes";
    public const string ReportsEntryLabel = "Reportes operativos";

    public static readonly IReadOnlyList<PortalAdminSection> AdminSections =
    [
        new PortalAdminSection(
            "Configuración Global",
            "Países, ciudades, moneda, datos demográficos, documentos, contacto, lealtad y métodos de pago.",
            ["country", "city", "currency", "gender", "nationality", "documenttype", "contacttype", "loyaltyprogram", "loyaltytier", "paymentmethod"]),
        new PortalAdminSection(
            "Gestión de Flota y Rutas",
            "Aerolínea, aeronaves, aeropuertos, terminales, puertas, rutas, vuelos base, horarios e incidencias.",
            ["airline", "aircraftmanufacturer", "aircrafttype", "aircraft", "airport", "terminal", "gate", "route", "baseflight", "routeschedule", "flightstatushistory", "delayreason", "flightdelay", "cancellationreason", "flightcancellation"]),
        new PortalAdminSection(
            "Comercial y Tarifas",
            "Precios por cabina, promociones, clases de cabina, mapas y asientos de vuelo, equipaje y tarifas.",
            ["flightcabinprice", "promotion", "flightpromotion", "cabinclass", "seatmap", "flightseat", "baggageallowance", "baggagetype", "faretype", "discounttype"]),
        new PortalAdminSection(
            "Seguridad y Personal",
            "Usuarios, roles, permisos, tripulación y empleados.",
            ["user", "role", "permission", "rolepermission", "crewrole", "jobposition", "employee", "flightcrew"]),
        new PortalAdminSection(
            "Diccionarios de Estados",
            "Todos los módulos cuyo título comienza por «Estados de…» (según catálogo de la aplicación).",
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
            "Consultar vuelos",
            "Consulta y gestión de vuelos programados (oferta operativa).",
            ["scheduledflight"]),
        new PortalClientSection(
            "Reservas y viajeros",
            "Reservas, pasajeros, clientes y descuentos aplicables.",
            ["reservation", "reservationdetail", "reservationstatushistory", "customer", "person", "passenger", "passengercontact", "passengerdiscount"]),
        new PortalClientSection(
            "Tiquetes, check-in y abordaje",
            "Emisión, equipaje asociado, check-in y pases de abordar.",
            ["ticket", "ticketstatushistory", "ticketbaggage", "checkin", "boardingpass"]),
        new PortalClientSection(
            "Pagos y reembolsos",
            "Registro de pagos y gestión de reembolsos.",
            ["payment", "refund"]),
        new PortalClientSection(
            "Programa de lealtad",
            "Cuentas y movimientos del pasajero frecuente.",
            ["loyaltyaccount", "loyaltytransaction"])
    ];
}

internal sealed record PortalClientSection(string Title, string Description, IReadOnlyList<string> ModuleKeys);

internal sealed record PortalAdminSection(string Title, string Description, IReadOnlyList<string> ModuleKeys);
