using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Infrastructure;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAnalytics.UI;


var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddSharedInfrastructure(builder.Configuration);
// Scoped: consumen IModuleUI, UseCases, DbContext, etc. (no pueden ser Singleton).
builder.Services.AddScoped<ReportsMenu>();
builder.Services.AddScoped<Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI.Client.ClientPortalApp>();
builder.Services.AddScoped<MainMenu>();

using var host = builder.Build();

await using (var scope = host.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await BootstrapDataSeeder.SeedAsync(db);

    var menu = scope.ServiceProvider.GetRequiredService<MainMenu>();
    await menu.RunAsync();
}

public sealed class MainMenu(
    IEnumerable<IModuleUI> modules,
    ReportsMenu reportsMenu,
    IConfiguration configuration,
    AppDbContext dbContext,
    Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI.Client.ClientPortalApp clientPortal)
{
    private readonly Dictionary<string, IModuleUI> _modules = BuildNormalizedModuleMap(modules);
    private readonly ReportsMenu _reportsMenu = reportsMenu;
    private readonly string _adminPin = configuration["AdminPortal:Pin"] ?? "0000";
    private readonly AuthService _authService = new(dbContext);
    private readonly AppDbContext _context = dbContext;
    private readonly Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI.Client.ClientPortalApp _clientPortal = clientPortal;

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
                    .Title("[bold yellow]Inicio — ¿a dónde desea ir?[/]")
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
        AnsiConsole.WriteLine();
        ConsoleDashboard.Info("Hasta luego. Gracias por usar el sistema.");
    }

    private static void RenderAccessPortal() => ConsoleDashboard.RenderAccessLanding();

    private async Task ShowAdminMenuAsync(CancellationToken cancellationToken)
    {
        if (!TryVerifyAdminPin(cancellationToken))
            return;

        while (!cancellationToken.IsCancellationRequested)
        {
            ConsoleDashboard.RenderAdminBackOfficeHeader(
                "Vuelos, flota, tarifas, personal, reportes y configuración.");
            ConsoleDashboard.NavigationHint();

            var sectionLabels = PortalAccess.AdminSections
                .Select((s, i) => new { Section = s, Label = $"{i + 1,2:00} — {s.Title}" })
                .ToList();

            var rootChoices = sectionLabels
                .Select(x => x.Label)
                .Append("99 — " + PortalAccess.ReportsEntryLabel)
                .Append(PortalAccess.BackToAccessMenu)
                .ToList();

            var rootChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold yellow]Área de administración — ¿dónde desea ir?[/]")
                    .PageSize(12)
                    .MoreChoicesText(ConsoleDashboard.SelectionMoreChoicesNav)
                    .AddChoices(rootChoices));

            if (rootChoice == PortalAccess.BackToAccessMenu)
                return;

            if (rootChoice == "99 — " + PortalAccess.ReportsEntryLabel)
            {
                await ConsoleErrorHandler.RunSafeAsync(ct => _reportsMenu.RunAsync(ct), cancellationToken);
                continue;
            }

            var selectedSection = sectionLabels.FirstOrDefault(s => s.Label == rootChoice);
            if (selectedSection is null)
            {
                ConsoleDashboard.Error("No se pudo abrir el área elegida.");
                PauseReturn();
                continue;
            }

            var section = selectedSection.Section;
            await RunAdminSectionAsync(section, cancellationToken);
        }
    }

    private async Task RunAdminSectionAsync(PortalAdminSection section, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            AnsiConsole.Clear();
            ConsoleDashboard.AdminSubScreenTitle(
                section.Title,
                section.Description);
            ConsoleDashboard.NavigationHint();

            var items = BuildOrderedModuleItems(section.ModuleKeys);

            if (items.Count == 0)
            {
                ConsoleDashboard.Warning("No hay módulos registrados en esta área.");
                PauseReturn();
                return;
            }

            var indexed = items
                .Select((x, i) => new { Item = x, Label = $"{i + 1,2:00} — {x.Title}" })
                .ToList();

            var selected = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold yellow]¿Qué módulo desea gestionar?[/]")
                    .PageSize(18)
                    .MoreChoicesText(ConsoleDashboard.SelectionMoreChoicesNav)
                    .AddChoices(indexed.Select(x => x.Label).Append(PortalAccess.BackToAdminRoot)));

            if (selected == PortalAccess.BackToAdminRoot)
                return;

            var picked = indexed.FirstOrDefault(x => x.Label == selected);
            if (picked is null)
            {
                ConsoleDashboard.Error("No se pudo abrir el módulo elegido.");
                PauseReturn();
                continue;
            }

            var item   = picked.Item;
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

        var loggedOut = await _clientPortal.RunAsync(cancellationToken);
        if (loggedOut)
            return;
    }

    private async Task<bool> ShowClientLoginAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            AnsiConsole.Clear();
            ConsoleDashboard.SubScreenTitle(
                "Portal de clientes — Acceso",
                "Inicie sesión o cree una cuenta si es la primera vez.");

            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold yellow]¿Cómo desea continuar?[/]")
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
        ConsoleDashboard.SubScreenTitle("Iniciar sesión", "Ingrese usuario y contraseña.");

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
            AnsiConsole.WriteLine();
            ConsoleDashboard.Error(result.ErrorMessage ?? "Credenciales incorrectas.");
            PauseReturn();
            return false;
        }

        AnsiConsole.WriteLine();
        ConsoleDashboard.Success($"Bienvenido, {result.Username ?? username}.");
        PauseReturn();
        return true;
    }

    private async Task<bool> ShowRegisterFormAsync(CancellationToken cancellationToken)
    {
        AnsiConsole.Clear();
        ConsoleDashboard.SubScreenTitle(
            "Crear cuenta nueva",
            "Complete el formulario. Los campos obligatorios deben rellenarse.");

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
            AnsiConsole.WriteLine();
            ConsoleDashboard.Error("Las contraseñas no coinciden.");
            PauseReturn();
            return false;
        }

        var firstName = AnsiConsole.Prompt(
            new TextPrompt<string>("[yellow]Nombre:[/]")
                .Validate(s => !string.IsNullOrWhiteSpace(s), "El nombre es obligatorio."));

        var lastName = AnsiConsole.Prompt(
            new TextPrompt<string>("[yellow]Apellido:[/]")
                .Validate(s => !string.IsNullOrWhiteSpace(s), "El apellido es obligatorio."));

        var docTypes = await _context.DocumentTypes.AsNoTracking().ToListAsync(cancellationToken);
        if (!docTypes.Any())
        {
            ConsoleDashboard.Error("No hay tipos de documento disponibles.");
            PauseReturn();
            return false;
        }

        var docTypeChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]Tipo de documento:[/]")
                .AddChoices(docTypes.Select(d => $"{d.DocumentTypeId} - {d.Name}")));

        if (!ConsoleIntPrompt.TryParseFirstSegmentAsInt32(docTypeChoice, out var documentTypeId))
        {
            ConsoleDashboard.Error("No se pudo interpretar el tipo de documento. Vuelva a intentar el registro.");
            PauseReturn();
            return false;
        }

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
            AnsiConsole.WriteLine();
            ConsoleDashboard.Error(result.ErrorMessage ?? "No se pudo registrar.");
            PauseReturn();
            return false;
        }

        AnsiConsole.WriteLine();
        ConsoleDashboard.Success($"Cuenta creada. Bienvenido, {result.Username}.");
        PauseReturn();
        return true;
    }

    private async Task RunModuleUiSafeAsync(IModuleUI module, string portalContext, string moduleTitle, CancellationToken cancellationToken)
    {
        AnsiConsole.Clear();
        await ConsoleErrorHandler.RunSafeAsync(ct => module.RunAsync(ct), cancellationToken);
    }

    private bool TryVerifyAdminPin(CancellationToken cancellationToken)
    {
        AnsiConsole.Clear();
        ConsoleDashboard.AdminSubScreenTitle(
            "Autenticación de administración",
            "PIN requerido. Deje vacío y Enter para volver al inicio.");

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

            ConsoleDashboard.Error("PIN incorrecto. Intente de nuevo.");
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
        AnsiConsole.WriteLine();
        ConsoleDashboard.FooterPressKey();
        Console.ReadKey(intercept: true);
    }

    private sealed record PortalMenuItem(string NormalizedKey, string Title);
}

public sealed class ReportsMenu(IServiceScopeFactory scopeFactory)
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            AnsiConsole.Clear();
            ConsoleDashboard.AdminSubScreenTitle(
                "Reportes",
                "Resúmenes y estadísticas (datos en tiempo real desde la base de datos).");
            ConsoleDashboard.NavigationHint();

            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold yellow]¿Qué reporte desea consultar?[/]")
                    .PageSize(12)
                    .MoreChoicesText(ConsoleDashboard.SelectionMoreChoicesNav)
                    .AddChoices(
                    [
                        "Reservas por estado",
                        "Vuelos con más reservas",
                        "Clientes con más reservas",
                        "Tiquetes emitidos por estado",
                        "Ingresos por estado de pago",
                        "Disponibilidad de asientos por vuelo",
                        "Reportes de fidelización (millas)",
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
                    case "Reportes de fidelización (millas)":
                        var analyticsUi = scope.ServiceProvider.GetRequiredService<LoyaltyAnalyticsConsoleUI>();
                        await analyticsUi.RunAsync(ct);
                        break;
                }
            }, cancellationToken);

            if (ok)
            {
                AnsiConsole.WriteLine();
                ConsoleDashboard.FooterPressKey();
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

        var table = ConsoleDashboard.NewDataTable();
        table.AddColumn("Vuelo");
        table.AddColumn("Fecha");
        table.AddColumn("Reservas");
        foreach (var row in data)
            table.AddRow(row.Vuelo, row.Fecha.ToString("yyyy-MM-dd"), row.Reservas.ToString());
        ConsoleDashboard.ShowWorkspaceTablePanel("Vuelos con más reservas (top 10)", table);
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

        var table = ConsoleDashboard.NewDataTable();
        table.AddColumn("Estado de pago");
        table.AddColumn("Monto total");
        foreach (var row in data)
            table.AddRow(row.Estado, row.Total.ToString("N2"));
        ConsoleDashboard.ShowWorkspaceTablePanel("Ingresos por estado de pago", table);
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

        var table = ConsoleDashboard.NewDataTable();
        table.AddColumn("Vuelo programado ID");
        table.AddColumn("Estado asiento");
        table.AddColumn("Cantidad");
        foreach (var row in data)
            table.AddRow(row.VueloProgramado.ToString(), row.EstadoAsiento, row.Cantidad.ToString());
        ConsoleDashboard.ShowWorkspaceTablePanel("Disponibilidad de asientos por vuelo (hasta 50 filas)", table);
    }

    private static void RenderSimpleTable(string title, string leftColumn, string rightColumn, IEnumerable<(string Left, string Right)> rows)
    {
        var table = ConsoleDashboard.NewDataTable();
        table.AddColumn(leftColumn);
        table.AddColumn(rightColumn);
        foreach (var row in rows)
            table.AddRow(Markup.Escape(row.Left), Markup.Escape(row.Right));

        ConsoleDashboard.ShowWorkspaceTablePanel(title, table);
    }
}

public static class FunctionalNavigation
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
        ["waitlist"] = "Lista de espera",
        ["reprogramminghistory"] = "Historial de reprogramaciones",
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
        ["boardingpass"] = "Pases de abordar",
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
        ["loyaltyanalytics"] = " Reportes de Fidelización (Millas)",
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

public static class PortalAccess
{
    public const string AdminPortalLabel  = "[[1]] Administración";
    public const string ClientPortalLabel = "[[2]] Portal de clientes";
    public const string BackToAccessMenu  = "« Volver";
    public const string BackToAdminRoot   = "« Volver";
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
            FunctionalNavigation.GetModuleKeysWithTitlePrefix("Estados de ")),
        new PortalAdminSection(
            "📊 Fidelización – Reportes",
            "Analítica avanzada del programa de millas: acumulación, redención y viajeros frecuentes.",
            ["loyaltyanalytics"]),
    ];

    public static readonly HashSet<string> ClientPortalNormalizedKeys =
    [
        "scheduledflight", "customer", "person", "passenger", "passengercontact",
        "reservation", "reservationdetail", "reservationstatushistory", "waitlist", "reprogramminghistory",
        "ticket", "ticketstatushistory", "ticketbaggage", "checkin", "boardingpass",
        "payment", "refund", "passengerdiscount", "loyaltyaccount", "loyaltytransaction"
    ];

    public static readonly IReadOnlyList<PortalClientSection> ClientSections =
    [
        new PortalClientSection(
            "Mis reservas",
            "Consulta y seguimiento de tus reservas, lista de espera e historial de reprogramaciones.",
            ["reservation", "waitlist", "reprogramminghistory", "reservationstatushistory", "reservationdetail", "passenger", "passengercontact", "passengerdiscount", "customer", "person"]),
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
            ["loyaltyaccount", "loyaltytransaction"]),
    ];
}

public sealed record PortalClientSection(string Title, string Description, IReadOnlyList<string> ModuleKeys);

public sealed record PortalAdminSection(string Title, string Description, IReadOnlyList<string> ModuleKeys);