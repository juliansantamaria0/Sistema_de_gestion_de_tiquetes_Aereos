using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Spectre.Console;

namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.UI;

public sealed class ReservationBookingWizardUI
{
    private readonly IReservationService _reservationService;
    private readonly AppDbContext        _context;

    public ReservationBookingWizardUI(IReservationService reservationService, AppDbContext context)
    {
        _reservationService = reservationService;
        _context            = context;
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await RunWizardAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            AnsiConsole.MarkupLine("[yellow]Operación cancelada.[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {Markup.Escape(ex.Message)}[/]");
            AnsiConsole.MarkupLine("[grey]Presiona una tecla para continuar...[/]");
            Console.ReadKey(intercept: true);
        }
    }

    private async Task RunWizardAsync(CancellationToken ct)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[green]Nueva Reserva[/]"));
        AnsiConsole.MarkupLine("[grey]Seleccione el vuelo y la cantidad de pasajeros.[/]");
        AnsiConsole.WriteLine();

        // ── Paso 1: cargar vuelos con asientos disponibles ───────────────────
        var flights = await LoadAvailableFlightsAsync(ct);

        if (flights.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No hay vuelos con asientos disponibles en este momento.[/]");
            AnsiConsole.MarkupLine("[grey]Presiona una tecla para continuar...[/]");
            Console.ReadKey(intercept: true);
            return;
        }

        // ── Paso 2: seleccionar vuelo ────────────────────────────────────────
        var flightLabels = flights
            .Select(f => $"{f.FlightCode}  {f.DepartureDate:yyyy-MM-dd} {f.DepartureTime:hh\\:mm}  " +
                         $"{f.Origin} -> {f.Destination}  ({f.AvailableSeats} asientos libres)")
            .ToList();

        const string cancelOption = "« Volver";
        var allChoices = flightLabels.Append(cancelOption).ToList();

        var selectedLabel = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]Seleccione un vuelo:[/]")
                .PageSize(12)
                .AddChoices(allChoices));

        if (selectedLabel == cancelOption)
            return;

        var flight = flights[flightLabels.IndexOf(selectedLabel)];

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"[green]✓[/] Vuelo: [bold]{Markup.Escape(flight.FlightCode)}[/]  " +
                               $"{flight.DepartureDate:yyyy-MM-dd} {flight.DepartureTime:hh\\:mm}  " +
                               $"{Markup.Escape(flight.Origin)} → {Markup.Escape(flight.Destination)}");
        AnsiConsole.MarkupLine($"  Asientos disponibles: [bold]{flight.AvailableSeats}[/]");
        AnsiConsole.WriteLine();

        // ── Paso 3: cantidad de pasajeros ────────────────────────────────────
        var passengerCount = AnsiConsole.Prompt(
            new TextPrompt<int>("[yellow]¿Cuántas personas viajarán? (incluyéndose):[/]")
                .Validate(n => n >= 1 && n <= flight.AvailableSeats,
                          $"Debe ser entre 1 y {flight.AvailableSeats}."));

        AnsiConsole.WriteLine();

        // ── Paso 4: obtener estado inicial ───────────────────────────────────
        var createdStatusId = await _context.ReservationStatuses.AsNoTracking()
            .Where(s => s.Name == "CREATED")
            .Select(s => s.Id)
            .FirstOrDefaultAsync(ct);

        if (createdStatusId == 0)
        {
            AnsiConsole.MarkupLine("[red]No se encontró el estado de reserva 'CREATED'. Contacte al administrador.[/]");
            AnsiConsole.MarkupLine("[grey]Presiona una tecla para continuar...[/]");
            Console.ReadKey(intercept: true);
            return;
        }

        // ── Paso 5: crear reservas ───────────────────────────────────────────
        var created = new List<ReservationDto>();

        await AnsiConsole.Status()
            .StartAsync("[yellow]Creando reservas...[/]", async ctx =>
            {
                ctx.Spinner(Spinner.Known.Dots);
                for (int i = 0; i < passengerCount; i++)
                {
                    ctx.Status($"[yellow]Creando reserva {i + 1} de {passengerCount}...[/]");
                    try
                    {
                        var dto = await _reservationService.CreateForCurrentUserAsync(
                            flight.ScheduledFlightId,
                            createdStatusId,
                            ct);
                        created.Add(dto);
                    }
                    catch (Exception ex)
                    {
                        AnsiConsole.MarkupLine($"[red]Error en reserva {i + 1}: {Markup.Escape(ex.Message)}[/]");
                    }
                }
            });

        AnsiConsole.WriteLine();

        // ── Paso 6: mostrar resultado ────────────────────────────────────────
        if (created.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No se pudo crear ninguna reserva.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[green]✓ {created.Count} reserva(s) creada(s) exitosamente![/]");
            AnsiConsole.WriteLine();

            var table = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("Pasajero")
                .AddColumn("Código de reserva")
                .AddColumn("Estado");

            for (int i = 0; i < created.Count; i++)
                table.AddRow($"Pasajero {i + 1}", created[i].ReservationCode, "Pendiente de confirmación");

            AnsiConsole.Write(table);
        }

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[grey]Presiona una tecla para continuar...[/]");
        Console.ReadKey(intercept: true);
    }

    private async Task<List<FlightInfoDto>> LoadAvailableFlightsAsync(CancellationToken ct)
    {
        // IDs de estado AVAILABLE
        var availableStatusId = await _context.SeatStatuses.AsNoTracking()
            .Where(s => s.Name == "AVAILABLE")
            .Select(s => s.Id)
            .FirstOrDefaultAsync(ct);

        if (availableStatusId == 0) return [];

        // Vuelos programados con su código de vuelo
        var scheduledFlights = await _context.ScheduledFlights.AsNoTracking()
            .Join(_context.BaseFlights.AsNoTracking(),
                sf => sf.BaseFlightId,
                bf => bf.Id,
                (sf, bf) => new { sf.Id, sf.BaseFlightId, bf.FlightCode, bf.RouteId,
                                   sf.DepartureDate, sf.DepartureTime })
            .ToListAsync(ct);

        if (scheduledFlights.Count == 0) return [];

        // Contar asientos disponibles por vuelo
        var flightIds = scheduledFlights.Select(f => f.Id).ToList();
        var seatCounts = await _context.FlightSeats.AsNoTracking()
            .Where(fs => flightIds.Contains(fs.ScheduledFlightId)
                      && fs.SeatStatusId == availableStatusId)
            .GroupBy(fs => fs.ScheduledFlightId)
            .Select(g => new { FlightId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.FlightId, x => x.Count, ct);

        // Info de rutas y aeropuertos (opcional — puede estar vacío)
        var routeIds = scheduledFlights.Select(f => f.RouteId).Distinct().ToList();
        var routes = await _context.Routes.AsNoTracking()
            .Where(r => routeIds.Contains(r.Id))
            .ToListAsync(ct);
        var routeMap = routes.ToDictionary(r => r.Id);

        var airportIds = routes
            .SelectMany(r => new[] { r.OriginAirportId, r.DestinationAirportId })
            .Distinct()
            .ToList();
        var airportMap = await _context.Airports.AsNoTracking()
            .Where(a => airportIds.Contains(a.AirportId))
            .ToDictionaryAsync(a => a.AirportId, a => a.IataCode, ct);

        // Ensamblar resultados solo con asientos disponibles
        return scheduledFlights
            .Select(f =>
            {
                var seats   = seatCounts.TryGetValue(f.Id, out var c) ? c : 0;
                var origin  = "—";
                var dest    = "—";
                if (routeMap.TryGetValue(f.RouteId, out var route))
                {
                    airportMap.TryGetValue(route.OriginAirportId,      out origin!);
                    airportMap.TryGetValue(route.DestinationAirportId, out dest!);
                    origin ??= "—";
                    dest   ??= "—";
                }
                return new FlightInfoDto(f.Id, f.FlightCode, origin, dest,
                                         f.DepartureDate, f.DepartureTime, seats);
            })
            .Where(f => f.AvailableSeats > 0)
            .OrderBy(f => f.DepartureDate)
            .ThenBy(f => f.DepartureTime)
            .ToList();
    }

    private sealed record FlightInfoDto(
        int      ScheduledFlightId,
        string   FlightCode,
        string   Origin,
        string   Destination,
        DateOnly DepartureDate,
        TimeOnly DepartureTime,
        int      AvailableSeats);
}
