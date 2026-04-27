namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.CheckIn.UI;

using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.BoardingPass.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CheckIn.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CheckIn.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Infrastructure;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class CheckInWizardUI
{
    private static readonly string[] AllowedTicketStatuses = new[] { "PAID", "PAGADO", "ISSUED" };
    private static readonly string[] EnabledFlightStatuses = new[] { "SCHEDULED", "BOARDING", "HABILITADO" };

    private readonly AppDbContext             _db;
    private readonly CheckInPassengerUseCase  _checkInPassenger;
    private readonly ICheckInService          _checkInService;
    private readonly IBoardingPassService     _boardingPassService;

    public CheckInWizardUI(
        AppDbContext            db,
        CheckInPassengerUseCase checkInPassenger,
        ICheckInService         checkInService,
        IBoardingPassService    boardingPassService)
    {
        _db                   = db;
        _checkInPassenger     = checkInPassenger;
        _checkInService       = checkInService;
        _boardingPassService  = boardingPassService;
    }

    public async Task RunCheckInAsync(CancellationToken cancellationToken = default)
    {
        AnsiConsole.Clear();
        ConsoleDashboard.SubScreenTitle(
            "Check-in del pasajero",
            "Confirme su llegada al aeropuerto y obtenga su pase de abordar.");
        ConsoleDashboard.NavigationHint();

        if (!CurrentUser.IsAuthenticated || !CurrentUser.CustomerId.HasValue)
        {
            ConsoleDashboard.Error("Debe iniciar sesión para hacer check-in.");
            return;
        }

        var customerId = CurrentUser.CustomerId.Value;

        try
        {
            var elegibles = await BuildEligibleTicketsAsync(customerId, cancellationToken);
            if (elegibles.Count == 0)
            {
                ConsoleDashboard.Warning(
                    "No hay tiquetes pagados pendientes de check-in en su cuenta. " +
                    "Revise primero Reservas → Mis reservas → confirme y pague el tiquete.");
                return;
            }

            var table = ConsoleDashboard.NewDataTable();
            table.AddColumn("ID");
            table.AddColumn("Cód. tiquete");
            table.AddColumn("Vuelo");
            table.AddColumn("Fecha");
            table.AddColumn("Asiento");
            table.AddColumn("Estado tiquete");
            foreach (var row in elegibles)
            {
                table.AddRow(
                    row.TicketId.ToString(),
                    Markup.Escape(row.TicketCode),
                    Markup.Escape(row.FlightCode),
                    row.DepartureDate.ToString("yyyy-MM-dd"),
                    row.SeatNumber is null ? "[grey]auto[/]" : Markup.Escape(row.SeatNumber),
                    Markup.Escape(row.TicketStatusName));
            }
            ConsoleDashboard.ShowTablePanel("Tiquetes elegibles para check-in", table);
            AnsiConsole.WriteLine();

            const string back = "« Volver (sin hacer check-in)";
            var labels = elegibles
                .Select(r => $"Tiquete {r.TicketCode}  ·  Vuelo {r.FlightCode}  ·  {r.DepartureDate:yyyy-MM-dd}  (id {r.TicketId})")
                .Append(back)
                .ToList();

            var pick = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold yellow]Seleccione el tiquete para realizar el check-in:[/]")
                    .PageSize(12)
                    .MoreChoicesText(ConsoleDashboard.SelectionMoreChoicesNav)
                    .AddChoices(labels));

            if (pick == back) return;

            var selected = elegibles[labels.IndexOf(pick)];

            var counterNumber = AnsiConsole.Prompt(
                new TextPrompt<string>("[yellow]Mostrador (opcional):[/]")
                    .AllowEmpty());

            if (!AnsiConsole.Confirm(
                    $"[yellow]¿Confirmar check-in para tiquete[/] [bold]{Markup.Escape(selected.TicketCode)}[/]?"))
            {
                ConsoleDashboard.Info("Operación cancelada.");
                return;
            }

            var result = await _checkInPassenger.ExecuteAsync(
                selected.TicketId,
                string.IsNullOrWhiteSpace(counterNumber) ? null : counterNumber,
                cancellationToken);

            AnsiConsole.WriteLine();
            ConsoleDashboard.Success(
                $"Check-in realizado. Pase de abordar: {result.BoardingPassCode}");
            AnsiConsole.WriteLine();

            await RenderBoardingPassByCheckInIdAsync(result.CheckInId, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            // Errores de negocio: el mensaje ya viene listo para el usuario.
            ConsoleDashboard.Error(ex.Message);
        }
        catch (OperationCanceledException)
        {
            // Cancelación cooperativa, no es un error que mostrar.
        }
        catch (Exception)
        {
            // Cualquier fallo inesperado (red, BD, nulos) se contiene aquí para no tumbar la app.
            ConsoleDashboard.Error("Ha ocurrido un error inesperado en el sistema. Por favor, intente más tarde.");
        }
    }

    public async Task RunViewBoardingPassAsync(CancellationToken cancellationToken = default)
    {
        AnsiConsole.Clear();
        ConsoleDashboard.SubScreenTitle(
            "Mis pases de abordar",
            "Visualice los pases emitidos para sus check-ins recientes.");
        ConsoleDashboard.NavigationHint();

        if (!CurrentUser.IsAuthenticated || !CurrentUser.CustomerId.HasValue)
        {
            ConsoleDashboard.Error("Debe iniciar sesión para consultar sus pases de abordar.");
            return;
        }

        try
        {
            var pases = (await _boardingPassService.GetAllAsync(cancellationToken)).ToList();
            if (pases.Count == 0)
            {
                ConsoleDashboard.Warning(
                    "No tiene pases de abordar emitidos. Realice primero el check-in en Check-in → Hacer check-in.");
                return;
            }

            var checkInIds = pases.Select(p => p.CheckInId).Distinct().ToList();
            var checkInById = await _db.CheckIns
                .AsNoTracking()
                .Where(c => checkInIds.Contains(c.Id))
                .ToDictionaryAsync(c => c.Id, cancellationToken);

            var table = ConsoleDashboard.NewDataTable();
            table.AddColumn("Cód. pase");
            table.AddColumn("Tiquete");
            table.AddColumn("Asiento");
            table.AddColumn("Puerta");
            table.AddColumn("Grupo");
            table.AddColumn("Emitido");

            foreach (var p in pases.OrderByDescending(x => x.IssuedAt))
            {
                var ticketLabel = "?";
                if (checkInById.TryGetValue(p.CheckInId, out var ci))
                {
                    var tCode = await _db.Tickets.AsNoTracking()
                        .Where(t => t.Id == ci.TicketId)
                        .Select(t => t.TicketCode)
                        .FirstOrDefaultAsync(cancellationToken);
                    ticketLabel = tCode ?? ci.TicketId.ToString();
                }

                table.AddRow(
                    Markup.Escape(p.BoardingPassCode),
                    Markup.Escape(ticketLabel),
                    p.FlightSeatId.ToString(),
                    p.GateId?.ToString() ?? "[grey]—[/]",
                    Markup.Escape(p.BoardingGroup ?? "—"),
                    p.IssuedAt.ToString("yyyy-MM-dd HH:mm"));
            }
            ConsoleDashboard.ShowTablePanel("Listado de pases", table);
            AnsiConsole.WriteLine();

            const string back = "« Volver";
            var labels = pases
                .OrderByDescending(p => p.IssuedAt)
                .Select(p => $"{p.BoardingPassCode}  ·  emitido {p.IssuedAt:yyyy-MM-dd HH:mm}")
                .Append(back)
                .ToList();

            var pick = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Seleccione un pase para verlo en detalle:[/]")
                    .PageSize(12)
                    .MoreChoicesText(ConsoleDashboard.SelectionMoreChoicesNav)
                    .AddChoices(labels));

            if (pick == back) return;

            var ordered = pases.OrderByDescending(p => p.IssuedAt).ToList();
            var index = labels.IndexOf(pick);
            if (index < 0 || index >= ordered.Count) return;
            var pase = ordered[index];
            await RenderBoardingPassByCheckInIdAsync(pase.CheckInId, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            ConsoleDashboard.Error(ex.Message);
        }
        catch (OperationCanceledException)
        {
            // Cancelación cooperativa.
        }
        catch (Exception)
        {
            ConsoleDashboard.Error("Ha ocurrido un error inesperado en el sistema. Por favor, intente más tarde.");
        }
    }

    private async Task RenderBoardingPassByCheckInIdAsync(
        int               checkInId,
        CancellationToken cancellationToken)
    {
        var pase = await _boardingPassService.GetByCheckInAsync(checkInId, cancellationToken);
        if (pase is null)
        {
            ConsoleDashboard.Error("No se encontró el pase de abordar asociado.");
            return;
        }

        var checkIn = await _db.CheckIns
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == checkInId, cancellationToken);
        if (checkIn is null)
        {
            ConsoleDashboard.Error("No se encontró el registro de check-in.");
            return;
        }

        var ticket = await _db.Tickets
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == checkIn.TicketId, cancellationToken);

        var detail = ticket is null ? null : await _db.ReservationDetails
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == ticket.ReservationDetailId, cancellationToken);

        var reservation = detail is null ? null : await _db.Reservations
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == detail.ReservationId, cancellationToken);

        var scheduledFlight = reservation is null ? null : await _db.ScheduledFlights
            .AsNoTracking()
            .FirstOrDefaultAsync(sf => sf.Id == reservation.ScheduledFlightId, cancellationToken);

        var baseFlight = scheduledFlight is null ? null : await _db.BaseFlights
            .AsNoTracking()
            .FirstOrDefaultAsync(bf => bf.Id == scheduledFlight.BaseFlightId, cancellationToken);

        var routeInfo = baseFlight is null
            ? null
            : await (
                from rt in _db.Routes.AsNoTracking()
                where rt.Id == baseFlight.RouteId
                join oa in _db.Airports.AsNoTracking() on rt.OriginAirportId equals oa.AirportId
                join da in _db.Airports.AsNoTracking() on rt.DestinationAirportId equals da.AirportId
                select new { Origin = oa.IataCode, Destination = da.IataCode, OriginName = oa.Name, DestinationName = da.Name }
            ).FirstOrDefaultAsync(cancellationToken);

        var seatNumber = await _db.FlightSeats.AsNoTracking()
            .Where(fs => fs.Id == pase.FlightSeatId)
            .Join(_db.SeatMaps.AsNoTracking(), fs => fs.SeatMapId, sm => sm.Id, (fs, sm) => sm.SeatNumber)
            .FirstOrDefaultAsync(cancellationToken) ?? "—";

        var gateCode = pase.GateId.HasValue
            ? await _db.Gates.AsNoTracking()
                .Where(g => g.GateId == pase.GateId.Value)
                .Select(g => g.Code)
                .FirstOrDefaultAsync(cancellationToken)
            : null;

        string passengerName = "—";
        if (detail is not null)
        {
            var passenger = await _db.Passengers
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == detail.PassengerId, cancellationToken);
            if (passenger is not null)
            {
                var person = await _db.Persons
                    .AsNoTracking()
                    .FirstOrDefaultAsync(per => per.Id == passenger.PersonId, cancellationToken);
                if (person is not null)
                    passengerName = ($"{person.FirstName} {person.LastName}").Trim();
            }
        }

        var header = new Markup(
            "[bold white on deepskyblue2]  PASE DE ABORDAR  [/]   " +
            "[grey]Boarding Pass[/]");
        AnsiConsole.Write(
            new Panel(header)
                .Border(BoxBorder.Double)
                .BorderStyle(Style.Parse("deepskyblue2"))
                .Expand());

        var origin = routeInfo?.Origin ?? "???";
        var destination = routeInfo?.Destination ?? "???";
        var route = new Markup(
            $"[bold white]{Markup.Escape(origin)}[/]  [grey]→[/]  [bold white]{Markup.Escape(destination)}[/]\n" +
            $"[dim]{Markup.Escape(routeInfo?.OriginName ?? string.Empty)}  →  {Markup.Escape(routeInfo?.DestinationName ?? string.Empty)}[/]");
        AnsiConsole.Write(
            new Panel(route)
                .Header("[seagreen1 bold]  Ruta  [/]")
                .Border(BoxBorder.Rounded)
                .BorderStyle(Style.Parse("grey"))
                .Padding(1, 0)
                .Expand());

        var grid = new Grid()
            .AddColumn(new GridColumn().NoWrap())
            .AddColumn();

        grid.AddRow("[aqua]Pasajero[/]",         Markup.Escape(passengerName));
        grid.AddRow("[aqua]Tiquete[/]",          Markup.Escape(ticket?.TicketCode ?? "—"));
        grid.AddRow("[aqua]Vuelo[/]",            Markup.Escape(baseFlight?.FlightCode ?? "—"));
        grid.AddRow("[aqua]Fecha[/]",            scheduledFlight?.DepartureDate.ToString("yyyy-MM-dd") ?? "—");
        grid.AddRow("[aqua]Hora salida[/]",      scheduledFlight?.DepartureTime.ToString("HH:mm")     ?? "—");
        grid.AddRow("[aqua]Asiento[/]",          $"[bold yellow]{Markup.Escape(seatNumber)}[/]");
        grid.AddRow("[aqua]Puerta[/]",           gateCode is null ? "[grey]por confirmar[/]" : $"[bold green]{Markup.Escape(gateCode)}[/]");
        grid.AddRow("[aqua]Grupo embarque[/]",   Markup.Escape(pase.BoardingGroup ?? "—"));
        grid.AddRow("[aqua]Código pase[/]",      $"[bold magenta]{Markup.Escape(pase.BoardingPassCode)}[/]");
        grid.AddRow("[aqua]Emitido[/]",          pase.IssuedAt.ToString("yyyy-MM-dd HH:mm"));

        AnsiConsole.Write(
            new Panel(grid)
                .Header("[slateblue1 bold]  Detalle del pase  [/]")
                .Border(BoxBorder.Rounded)
                .BorderStyle(Style.Parse("grey"))
                .Padding(1, 0)
                .Expand());

        AnsiConsole.MarkupLine(
            "[dim]Conserve este pase. Embarque cierra 15 min antes de la hora de salida.[/]");
    }

    private async Task<IReadOnlyList<EligibleTicketRow>> BuildEligibleTicketsAsync(
        int               customerId,
        CancellationToken cancellationToken)
    {
        // Left join contra flight_seat y seat_map: si el detalle no tiene asiento (FlightSeatId == 0
        // o no coincide con ningún registro), el tiquete sigue apareciendo y el UseCase asignará uno.
        var query =
            from t   in _db.Tickets.AsNoTracking()
            join ts  in _db.TicketStatuses.AsNoTracking()    on t.TicketStatusId       equals ts.Id
            join d   in _db.ReservationDetails.AsNoTracking() on t.ReservationDetailId  equals d.Id
            join r   in _db.Reservations.AsNoTracking()      on d.ReservationId        equals r.Id
            join sf  in _db.ScheduledFlights.AsNoTracking()  on r.ScheduledFlightId    equals sf.Id
            join bf  in _db.BaseFlights.AsNoTracking()       on sf.BaseFlightId        equals bf.Id
            join fst in _db.FlightStatuses.AsNoTracking()    on sf.FlightStatusId      equals fst.Id
            join fs  in _db.FlightSeats.AsNoTracking()       on d.FlightSeatId         equals fs.Id   into fsj
            from fs  in fsj.DefaultIfEmpty()
            join sm  in _db.SeatMaps.AsNoTracking()          on (fs != null ? fs.SeatMapId : -1) equals sm.Id into smj
            from sm  in smj.DefaultIfEmpty()
            where r.CustomerId == customerId
               && AllowedTicketStatuses.Contains(ts.Name)
               && !_db.CheckIns.Any(c => c.TicketId == t.Id)
            select new
            {
                t.Id,
                t.TicketCode,
                FlightCode       = bf.FlightCode,
                sf.DepartureDate,
                SeatNumber       = sm != null ? sm.SeatNumber : null,
                TicketStatusName = ts.Name,
                FlightStatusName = fst.Name
            };

        var raw = await query.ToListAsync(cancellationToken);

        return raw
            .Where(x => EnabledFlightStatuses.Contains(x.FlightStatusName, StringComparer.OrdinalIgnoreCase))
            .Select(x => new EligibleTicketRow(
                x.Id,
                x.TicketCode,
                x.FlightCode,
                x.DepartureDate,
                x.SeatNumber,
                x.TicketStatusName,
                x.FlightStatusName))
            .OrderBy(x => x.DepartureDate)
            .ToList();
    }

    private sealed record EligibleTicketRow(
        int      TicketId,
        string   TicketCode,
        string   FlightCode,
        DateOnly DepartureDate,
        string?  SeatNumber,
        string   TicketStatusName,
        string   FlightStatusName);
}
