using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;
using Spectre.Console;

namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.UI;

public sealed class ReservationBookingWizardUI
{
    private readonly IReservationService _reservationService;
    private readonly IPassengerService   _passengerService;
    private readonly AppDbContext        _context;

    public ReservationBookingWizardUI(
        IReservationService reservationService,
        IPassengerService   passengerService,
        AppDbContext        context)
    {
        _reservationService = reservationService;
        _passengerService   = passengerService;
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
            ConsoleDashboard.Error(ex.Message);
            AnsiConsole.WriteLine();
            ConsoleDashboard.FooterPressKey();
            Console.ReadKey(intercept: true);
        }
    }

    private async Task RunWizardAsync(CancellationToken ct)
    {
        AnsiConsole.Clear();
        ConsoleDashboard.Step(
            1,
            "Nueva reserva",
            "Elija un vuelo. Si no hay cupos, podrá inscribirse en lista de espera (pasajero + tarifa).");

        var flights = await LoadFlightsWithAvailabilityAsync(includeZeroSeats: true, ct);

        if (flights.Count == 0)
        {
            ConsoleDashboard.Warning("No hay vuelos programados con oferta en este momento.");
            AnsiConsole.WriteLine();
            ConsoleDashboard.FooterPressKey();
            Console.ReadKey(intercept: true);
            return;
        }

        // Sin markup en las opciones: SelectionPrompt debe comparar texto plano.
        var flightLabels = flights
            .Select(f =>
            {
                var cupoTxt = f.AvailableSeats == 0
                    ? "0 — LISTA DE ESPERA"
                    : f.AvailableSeats.ToString();
                return $"{f.FlightCode}  {f.DepartureDate:yyyy-MM-dd} {f.DepartureTime:hh\\:mm}  " +
                       $"{f.Origin} -> {f.Destination}  ·  Libres: {cupoTxt}  ·  #{f.ScheduledFlightId}";
            })
            .ToList();

        const string cancelOption = "« Volver";
        var allChoices = flightLabels.Append(cancelOption).ToList();

        var selectedLabel = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]Seleccione un vuelo (código, fecha y ruta):[/]")
                .PageSize(14)
                .AddChoices(allChoices));

        if (selectedLabel == cancelOption)
            return;

        var flight = flights[flightLabels.IndexOf(selectedLabel)];

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"[green]✓[/] Vuelo: [bold]{Markup.Escape(flight.FlightCode)}[/]  " +
                               $"{flight.DepartureDate:yyyy-MM-dd} {flight.DepartureTime:hh\\:mm}  " +
                               $"{Markup.Escape(flight.Origin)} → {Markup.Escape(flight.Destination)}");
        if (flight.AvailableSeats == 0)
            AnsiConsole.MarkupLine("  [yellow]Sin cupo: siguiente paso = lista de espera.[/]");
        else
            AnsiConsole.MarkupLine($"  Asientos disponibles: [bold]{flight.AvailableSeats}[/]");
        AnsiConsole.WriteLine();

        if (flight.AvailableSeats == 0)
        {
            await RunWaitlistBookingAsync(flight, ct);
            return;
        }

        AnsiConsole.MarkupLine(
            "[grey]Importante:[/] este asistente crea reservas en estado [bold]CREATED[/]. " +
            "Después deberá asignar [bold]asiento y tarifa[/] (Reservas → Asignar asiento y tarifa) " +
            "y luego [bold]confirmar[/] (Reservas → Mis reservas → Abrir: Reservas).");
        AnsiConsole.WriteLine();
        var passengerCount = AnsiConsole.Prompt(
            new TextPrompt<int>("[yellow]¿Cuántas plazas desea reservar en este vuelo?[/] [grey](típico: 1)[/]:")
                .Validate(n => n >= 1 && n <= flight.AvailableSeats,
                          $"Escriba un número entre 1 y {flight.AvailableSeats}."));

        AnsiConsole.WriteLine();

        var createdStatusId = await _context.ReservationStatuses.AsNoTracking()
            .Where(s => s.Name == "CREATED")
            .Select(s => s.Id)
            .FirstOrDefaultAsync(ct);

        if (createdStatusId == 0)
        {
            AnsiConsole.MarkupLine("[red]No se encontró el estado de reserva 'CREATED'. Contacte al administrador.[/]");
            AnsiConsole.MarkupLine("[grey]Presione una tecla para continuar...[/]");
            Console.ReadKey(intercept: true);
            return;
        }

        var created = new List<ReservationDto>();

        await AnsiConsole.Status()
            .StartAsync("[yellow]Creando reservas...[/]", async ctx =>
            {
                ctx.Spinner(Spinner.Known.Dots);
                for (var i = 0; i < passengerCount; i++)
                {
                    ctx.Status($"[yellow]Creando reserva {i + 1} de {passengerCount}...[/]");
                    try
                    {
                        var dto = await _reservationService.CreateForCurrentUserAsync(
                            flight.ScheduledFlightId,
                            createdStatusId,
                            requireAvailableSeats: true,
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

        if (created.Count == 0)
        {
            ConsoleDashboard.Error("No se pudo crear ninguna reserva.");
        }
        else
        {
            ConsoleDashboard.Success(
                $"{created.Count} reserva(s) creada(s). " +
                "Siguiente: Reservas → Asignar asiento y tarifa; luego Reservas → Mis reservas → Abrir: Reservas (confirmar).");
            AnsiConsole.WriteLine();

            var table = ConsoleDashboard.NewDataTable()
                .AddColumn("Pasajero")
                .AddColumn("Código de reserva")
                .AddColumn("Estado");

            for (var i = 0; i < created.Count; i++)
                table.AddRow($"Pasajero {i + 1}", created[i].ReservationCode, "Pendiente de confirmación");

            ConsoleDashboard.ShowTablePanel("Reservas creadas", table);
        }

        AnsiConsole.WriteLine();
        ConsoleDashboard.FooterPressKey();
        Console.ReadKey(intercept: true);
    }

    private async Task RunWaitlistBookingAsync(FlightInfoDto flight, CancellationToken ct)
    {
        if (!AnsiConsole.Confirm(
                "[yellow]Este vuelo no tiene asientos libres. ¿Desea solicitar ingreso a [bold]lista de espera[/]?[/]"))
            return;

        var passengers = (await _passengerService.GetAllAsync(ct)).ToList();
        if (passengers.Count == 0)
        {
            ConsoleDashboard.Error("No hay pasajeros asociados a su cuenta. Use el módulo Pasajeros.");
            AnsiConsole.WriteLine();
            ConsoleDashboard.FooterPressKey();
            Console.ReadKey(intercept: true);
            return;
        }

        var pax = passengers[0];
        if (passengers.Count > 1)
        {
            var pLabels = passengers
                .Select(p => $"Pasajero #{p.Id} (persona #{p.PersonId})")
                .ToList();
            var pPick = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]¿Qué pasajero va en lista de espera?[/]")
                    .PageSize(8)
                    .AddChoices(pLabels));
            pax = passengers[pLabels.IndexOf(pPick)];
        }

        var fareOptions = await (
            from fcp in _context.FlightCabinPrices.AsNoTracking()
            where fcp.ScheduledFlightId == flight.ScheduledFlightId
            join ft in _context.FareTypes.AsNoTracking() on fcp.FareTypeId equals ft.Id
            join cc in _context.CabinClasses.AsNoTracking() on fcp.CabinClassId equals cc.Id
            select new { fcp.FareTypeId, ft.Name, fcp.Price, Cabin = cc.Name }
        ).Distinct()
         .OrderBy(x => x.Price)
         .ToListAsync(ct);

        if (fareOptions.Count == 0)
        {
            ConsoleDashboard.Error("No hay tipos de tarifa para este vuelo. Un administrador debe cargar precios.");
            Console.ReadKey(intercept: true);
            return;
        }

        var fLabels = fareOptions
            .Select(f => $"{f.Name} ({f.Cabin}) — {f.Price.ToString("N2", System.Globalization.CultureInfo.InvariantCulture)}")
            .ToList();
        var fBack = "« Volver";
        var fPick = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]Tipo de tarifa solicitado (según cabina/tarifa del vuelo):[/]")
                .PageSize(12)
                .AddChoices(fLabels.Append(fBack)));
        if (fPick == fBack)
            return;
        var fare = fareOptions[fLabels.IndexOf(fPick)];

        ConsoleDashboard.Info("Registrando solicitud (transacción)…");
        var list = await _reservationService.CrearSolicitudListaEsperaAsync(
            flight.ScheduledFlightId,
            [(pax.Id, fare.FareTypeId)],
            ct);

        AnsiConsole.WriteLine();
        ConsoleDashboard.Success(
            $"Lista de espera: código {list[0].ReservationCode} (WAITLIST). Promoción automática si se libera cupo.");
        AnsiConsole.WriteLine();
        ConsoleDashboard.FooterPressKey();
        Console.ReadKey(intercept: true);
    }

    private async Task<List<FlightInfoDto>> LoadFlightsWithAvailabilityAsync(bool includeZeroSeats, CancellationToken ct)
    {
        var availableStatusId = await _context.SeatStatuses.AsNoTracking()
            .Where(s => s.Name == "AVAILABLE")
            .Select(s => s.Id)
            .FirstOrDefaultAsync(ct);

        if (availableStatusId == 0) return [];

        var scheduledFlights = await _context.ScheduledFlights.AsNoTracking()
            .Join(_context.BaseFlights.AsNoTracking(),
                sf => sf.BaseFlightId,
                bf => bf.Id,
                (sf, bf) => new { sf.Id, sf.BaseFlightId, bf.FlightCode, bf.RouteId,
                    sf.DepartureDate, sf.DepartureTime })
            .ToListAsync(ct);

        if (scheduledFlights.Count == 0) return [];

        var flightIds = scheduledFlights.Select(f => f.Id).ToList();
        var seatCounts = await _context.FlightSeats.AsNoTracking()
            .Where(fs => flightIds.Contains(fs.ScheduledFlightId)
                      && fs.SeatStatusId == availableStatusId)
            .GroupBy(fs => fs.ScheduledFlightId)
            .Select(g => new { FlightId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.FlightId, x => x.Count, ct);

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

        var list = scheduledFlights
            .Select(f =>
            {
                var seats = seatCounts.TryGetValue(f.Id, out var c) ? c : 0;
                var origin = "—";
                var dest = "—";
                if (routeMap.TryGetValue(f.RouteId, out var route))
                {
                    airportMap.TryGetValue(route.OriginAirportId, out origin!);
                    airportMap.TryGetValue(route.DestinationAirportId, out dest!);
                    origin ??= "—";
                    dest ??= "—";
                }
                return new FlightInfoDto(f.Id, f.FlightCode, origin, dest,
                    f.DepartureDate, f.DepartureTime, seats);
            })
            .Where(f => includeZeroSeats || f.AvailableSeats > 0)
            .OrderBy(f => f.DepartureDate)
            .ThenBy(f => f.DepartureTime)
            .ToList();

        return list;
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
