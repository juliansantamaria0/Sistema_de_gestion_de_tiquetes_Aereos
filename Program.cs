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
    var menu = scope.ServiceProvider.GetRequiredService<MainMenu>();
    await menu.RunAsync();
}

internal sealed class MainMenu(IEnumerable<IModuleUI> modules, ReportsMenu reportsMenu)
{
    private readonly Dictionary<string, IModuleUI> _modules = modules.ToDictionary(m => m.Key, StringComparer.OrdinalIgnoreCase);
    private readonly ReportsMenu _reportsMenu = reportsMenu;

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            RenderHome();

            var selectedCategory = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Seleccione una sección[/]")
                    .PageSize(12)
                    .AddChoices(FunctionalNavigation.Categories.Select(c => c.Name).Append("Salir")));

            if (selectedCategory == "Salir")
                break;

            if (selectedCategory == FunctionalNavigation.ReportsCategory)
            {
                await _reportsMenu.RunAsync(cancellationToken);
                continue;
            }

            var category = FunctionalNavigation.Categories.First(c => c.Name == selectedCategory);
            await RunCategoryAsync(category, cancellationToken);
        }

        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[green]Hasta luego.[/]");
    }

    private async Task RunCategoryAsync(MenuCategory category, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            AnsiConsole.Clear();
            RenderHeader($"Menú principal > {category.Name}", category.Description);

            var availableItems = category.ModuleKeys
                .Where(k => _modules.ContainsKey(k))
                .Select(k => new MenuItem(k, FunctionalNavigation.GetModuleTitle(k)))
                .ToList();

            if (availableItems.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No hay módulos registrados para esta sección.[/]");
                AnsiConsole.MarkupLine("[grey]Presiona una tecla para volver...[/]");
                Console.ReadKey(intercept: true);
                return;
            }

            var selected = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[yellow]{category.Name}[/]")
                    .PageSize(18)
                    .AddChoices(availableItems.Select(x => x.Title).Append("Volver")));

            if (selected == "Volver")
                return;

            var item = availableItems.First(x => x.Title == selected);
            var module = _modules[item.Key];

            AnsiConsole.Clear();
            RenderHeader($"Menú principal > {category.Name} > {item.Title}", "Gestión del módulo");

            try
            {
                await module.RunAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything | ExceptionFormats.ShowLinks);
                AnsiConsole.MarkupLine("[grey]Presiona una tecla para continuar...[/]");
                Console.ReadKey(intercept: true);
            }
        }
    }

    private static void RenderHome()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(
            new FigletText("Air Tickets")
                .Centered()
                .Color(Color.CornflowerBlue));

        AnsiConsole.MarkupLine("[grey]Sistema de gestión de tiquetes aéreos[/]");
        AnsiConsole.Write(new Rule("[green]Flujo funcional[/]"));
        AnsiConsole.MarkupLine("[silver]Use el menú por procesos del negocio: configuración, vuelos, clientes, reservas, tiquetes, pagos, consultas y reportes.[/]");
    }

    private static void RenderHeader(string breadcrumb, string description)
    {
        AnsiConsole.Write(new Rule($"[green]{Markup.Escape(breadcrumb)}[/]"));
        AnsiConsole.MarkupLine($"[grey]{Markup.Escape(description)}[/]");
        AnsiConsole.WriteLine();
    }

    private sealed record MenuItem(string Key, string Title);
}

internal sealed class ReportsMenu(IServiceScopeFactory scopeFactory)
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[green]Menú principal > Reportes[/]"));
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

            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                switch (option)
                {
                    case "Reservas por estado":
                        await RenderReservationsByStatusAsync(db, cancellationToken);
                        break;
                    case "Vuelos con más reservas":
                        await RenderFlightsWithMostReservationsAsync(db, cancellationToken);
                        break;
                    case "Clientes con más reservas":
                        await RenderTopCustomersAsync(db, cancellationToken);
                        break;
                    case "Tiquetes emitidos por estado":
                        await RenderTicketsByStatusAsync(db, cancellationToken);
                        break;
                    case "Ingresos por estado de pago":
                        await RenderRevenueByPaymentStatusAsync(db, cancellationToken);
                        break;
                    case "Disponibilidad de asientos por vuelo":
                        await RenderSeatAvailabilityAsync(db, cancellationToken);
                        break;
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]{Markup.Escape(ex.Message)}[/]");
            }

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[grey]Presiona una tecla para continuar...[/]");
            Console.ReadKey(intercept: true);
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

internal sealed record MenuCategory(string Name, string Description, IReadOnlyList<string> ModuleKeys);

internal static class FunctionalNavigation
{
    public const string ReportsCategory = "Reportes";

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
        ["statusId"] = "ReservationStatus",
    };

    public static IReadOnlyList<MenuCategory> Categories { get; } =
    [
        new(
            "Configuración operativa",
            "Registre aerolíneas, aeropuertos, destinos, aeronaves, rutas y catálogos base.",
            [
                "airline", "country", "city", "airport", "terminal", "gate",
                "aircraftmanufacturer", "aircrafttype", "aircraft",
                "route", "baseflight", "routeschedule", "flightstatus",
                "cabinclass", "seatstatus", "seatmap", "flightcabinprice",
                "currency", "paymentmethod", "paymentstatus", "refundstatus",
                "reservationstatus", "ticketstatus", "documenttype", "gender",
                "nationality", "contacttype", "faretype", "discounttype",
                "promotion", "flightpromotion", "baggageallowance", "baggagetype",
                "delayreason", "cancellationreason", "checkinstatus"
            ]),
        new(
            "Vuelos",
            "Cree vuelos con ruta completa, capacidad, horarios, tripulación e incidencias operativas.",
            [
                "baseflight", "scheduledflight", "route", "routeschedule", "flightstatus",
                "flightstatushistory", "flightseat", "flightcabinprice", "gate", "terminal",
                "aircraft", "aircrafttype", "flightcrew", "crewrole", "employee",
                "jobposition", "flightdelay", "flightcancellation"
            ]),
        new(
            "Clientes y pasajeros",
            "Registre clientes, personas, pasajeros, contactos y programa de lealtad.",
            [
                "person", "customer", "passenger", "passengercontact",
                "documenttype", "gender", "nationality", "contacttype",
                "loyaltyprogram", "loyaltytier", "loyaltyaccount", "loyaltytransaction"
            ]),
        new(
            "Reservas",
            "Cree reservas asociadas a cliente y vuelo, gestione estados y su trazabilidad.",
            [
                "reservation", "reservationdetail", "reservationstatus", "reservationstatushistory",
                "scheduledflight", "customer"
            ]),
        new(
            "Tiquetes y check-in",
            "Emita tiquetes desde reservas válidas, gestione equipaje, check-in y abordaje.",
            [
                "ticket", "ticketstatus", "ticketstatushistory", "ticketbaggage",
                "baggageallowance", "baggagetype", "checkin", "checkinstatus", "boardingpass"
            ]),
        new(
            "Pagos y reembolsos",
            "Registre pagos asociados a reservas o tiquetes y gestione reembolsos.",
            [
                "payment", "paymentstatus", "paymentmethod", "currency", "refund", "refundstatus"
            ]),
        new(
            "Consultas",
            "Consulte vuelos, reservas, clientes, tiquetes, pagos y trazabilidad desde consola.",
            [
                "scheduledflight", "reservation", "customer", "ticket", "payment",
                "reservationstatushistory", "ticketstatushistory", "flightstatushistory",
                "boardingpass", "checkin"
            ]),
        new(
            ReportsCategory,
            "Reportes operativos con LINQ.",
            [])
    ];

    public static string GetModuleTitle(string key)
        => ModuleTitles.TryGetValue(key, out var title) ? title : key;
}