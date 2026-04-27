using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReprogrammingHistory.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Waitlist.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Infrastructure;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

namespace Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI.Client;

/// <summary>
/// Portal de clientes guiado (menú claro + flujos paso a paso) para reducir fricción vs. módulos CRUD.
/// </summary>
public sealed class ClientPortalApp
{
    private readonly IServiceScopeFactory _scopeFactory;

    private const string LabelLogout  = "Cerrar sesión (salir de la cuenta)";
    private const string LabelBack   = "« Volver al menú principal (sin cerrar sesión)";
    private const string LabelBackCategory = "« Volver (categorías)";

    private enum MainMenuAction
    {
        // Reservas (flujo principal)
        Book,
        AssignSeat,
        MyReservations,

        // Seguimiento
        Waitlist,
        ReproHistory,

        // Cuenta/ayuda
        Help,

        // Avanzado
        Legacy
    }

    private enum MainCategory
    {
        Reservations,
        Tracking,
        HelpAndAccount,
        Advanced
    }

    private static readonly (MainCategory Category, string Label)[] MainCategories =
    [
        (MainCategory.Reservations,  "1) Reservas (reservar, asiento/tarifa, mis reservas)"),
        (MainCategory.Tracking,      "2) Seguimiento (lista de espera, historial de cambios)"),
        (MainCategory.HelpAndAccount,"3) Ayuda y cuenta"),
        (MainCategory.Advanced,      "4) Avanzado (módulos y catálogos)")
    ];

    private static readonly (MainMenuAction Action, string Label)[] ReservationActions =
    [
        (MainMenuAction.Book,           "Reservar un vuelo (asistente)"),
        (MainMenuAction.AssignSeat,     "Asignar asiento y tarifa (asistente)"),
        (MainMenuAction.MyReservations, "Mis reservas (confirmar, cancelar, reprogramar)")
    ];

    private static readonly (MainMenuAction Action, string Label)[] TrackingActions =
    [
        (MainMenuAction.Waitlist,     "Lista de espera (por vuelo)"),
        (MainMenuAction.ReproHistory, "Historial de reprogramaciones")
    ];

    private static readonly (MainMenuAction Action, string Label)[] HelpAndAccountActions =
    [
        (MainMenuAction.Help, "Ayuda (flujo, preguntas frecuentes)")
    ];

    private static readonly (MainMenuAction Action, string Label)[] AdvancedActions =
    [
        (MainMenuAction.Legacy, "Más opciones (módulos avanzados)")
    ];

    public ClientPortalApp(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<bool> RunAsync(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await RenderHeaderAsync(cancellationToken);

            var mainChoices = MainCategories
                .Select(m => m.Label)
                .Append(LabelLogout)
                .Append(LabelBack)
                .ToList();

            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold yellow]Menú principal — ¿qué desea hacer?[/]")
                    .PageSize(12)
                    .MoreChoicesText(ConsoleDashboard.SelectionMoreChoicesNav)
                    .AddChoices(mainChoices));

            if (option == LabelBack)
                return false;

            if (option == LabelLogout)
            {
                AuthService.Logout();
                AnsiConsole.Clear();
                ConsoleDashboard.Success("Sesión cerrada. Hasta pronto.");
                AnsiConsole.WriteLine();
                PressAnyKey();
                return true;
            }

            if (!MainCategories.Any(m => m.Label == option))
                throw new InvalidOperationException("Opción de menú no reconocida.");
            var category = MainCategories.First(m => m.Label == option).Category;
            var action = await PickActionInCategoryAsync(category, cancellationToken);
            if (action is null)
                continue;

            var ok = await ConsoleErrorHandler.TryRunSafeAsync(async ct =>
            {
                switch (action.Value)
                {
                    case MainMenuAction.Book:           await RunBookingWizardAsync(ct); break;
                    case MainMenuAction.AssignSeat:     await RunAssignSeatAndFareFlowAsync(ct); break;
                    case MainMenuAction.MyReservations: await RunMyReservationsHubAsync(ct); break;
                    case MainMenuAction.Waitlist:       await RunWaitlistLookupAsync(ct); break;
                    case MainMenuAction.ReproHistory:   await RunReprogrammingHistoryLookupAsync(ct); break;
                    case MainMenuAction.Help:            ShowHelp(); break;
                    case MainMenuAction.Legacy:         await RunLegacyAdvancedAsync(ct); break;
                    default: throw new InvalidOperationException();
                }
            }, cancellationToken);

            if (ok)
                OfferPauseAfterActionAsync(action.Value);
        }
        return false;
    }

    private async Task<MainMenuAction?> PickActionInCategoryAsync(MainCategory category, CancellationToken ct)
    {
        AnsiConsole.WriteLine();
        var list = category switch
        {
            MainCategory.Reservations  => ReservationActions,
            MainCategory.Tracking      => TrackingActions,
            MainCategory.HelpAndAccount=> HelpAndAccountActions,
            _                          => AdvancedActions
        };

        var title = category switch
        {
            MainCategory.Reservations   => "Reservas",
            MainCategory.Tracking       => "Seguimiento",
            MainCategory.HelpAndAccount => "Ayuda y cuenta",
            _                           => "Avanzado"
        };

        var hint = category switch
        {
            MainCategory.Reservations =>
                "Paso a paso para reservar y gestionar sus reservas (asiento, confirmación, cancelación, reprogramación).",
            MainCategory.Tracking =>
                "Consultar lista de espera por vuelo e historial de reprogramaciones.",
            MainCategory.HelpAndAccount =>
                "Guía del sistema y acciones de cuenta.",
            _ =>
                "Pantallas avanzadas tipo catálogo/CRUD (algunos títulos pueden estar en inglés)."
        };

        AnsiConsole.Clear();
        ConsoleDashboard.SubScreenTitle(title, hint);
        ConsoleDashboard.NavigationHint();

        var choices = list.Select(x => x.Label).Append(LabelBackCategory).ToList();
        var pick = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]Seleccione una opción:[/]")
                .PageSize(12)
                .MoreChoicesText(ConsoleDashboard.SelectionMoreChoicesNav)
                .AddChoices(choices));

        if (pick == LabelBackCategory)
            return null;

        if (!list.Any(x => x.Label == pick))
        {
            ConsoleDashboard.Error("No se pudo leer la opción elegida.");
            PressAnyKey();
            return null;
        }

        // Evitar warning por async: el método es async para permitir futuras cargas.
        await Task.CompletedTask;
        return list.First(x => x.Label == pick).Action;
    }

    private static void OfferPauseAfterActionAsync(MainMenuAction last)
    {
        // Evita doble pausa: Ayuda, lista de espera e historial ya piden tecla; «Mis reservas» es un bucle propio.
        if (last is MainMenuAction.Help or MainMenuAction.Waitlist or MainMenuAction.ReproHistory
            or MainMenuAction.MyReservations)
            return;
        PressAnyKey();
    }

    private async Task RenderHeaderAsync(CancellationToken cancellationToken = default)
    {
        ConsoleDashboard.RenderClientPortalHeader(
            "Flujo recomendado: Reservar → Asignar asiento y tarifa → Mis reservas (confirmar/cancelar/reprogramar).");
        await TryWriteSessionSummaryAsync(cancellationToken);
        ConsoleDashboard.NavigationHint();
    }

    private async Task TryWriteSessionSummaryAsync(CancellationToken ct)
    {
        if (!CurrentUser.IsAuthenticated
            || !CurrentUser.CustomerId.HasValue
            || !CurrentUser.PersonId.HasValue)
            return;

        try
        {
            await using var scope  = _scopeFactory.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var name = await db.Persons.AsNoTracking()
                .Where(p => p.Id == CurrentUser.PersonId.Value)
                .Select(p => p.FirstName + " " + p.LastName)
                .FirstOrDefaultAsync(ct);
            var display = string.IsNullOrWhiteSpace(name) ? (CurrentUser.Username ?? "Usuario") : name!.Trim();
            var email   = await db.Customers.AsNoTracking()
                .Where(c => c.Id == CurrentUser.CustomerId.Value)
                .Select(c => c.Email)
                .FirstOrDefaultAsync(ct);

            AnsiConsole.MarkupLine(
                "[grey]Sesión:[/] [white]" + Markup.Escape(display) + "[/]" +
                (string.IsNullOrWhiteSpace(email) ? string.Empty : "  [grey]·[/]  [dim]" + Markup.Escape(email!) + "[/]"));

            var res = await db.Reservations.AsNoTracking()
                .Where(r => r.CustomerId == CurrentUser.CustomerId.Value && r.CancelledAt == null)
                .Select(r => new { r.Id, r.ReservationStatusId, r.ConfirmedAt })
                .ToListAsync(ct);
            if (res.Count == 0)
            {
                AnsiConsole.MarkupLine("[dim]Resumen: sin reservas activas.[/]");
                return;
            }

            var st = await db.ReservationStatuses.AsNoTracking()
                .ToDictionaryAsync(s => s.Id, s => s.Name, ct);
            var rIds = res.Select(x => x.Id).ToList();
            var dCount = await db.ReservationDetails.AsNoTracking()
                .Where(d => rIds.Contains(d.ReservationId))
                .GroupBy(d => d.ReservationId)
                .ToDictionaryAsync(g => g.Key, g => g.Count(), ct);

            var nCreated = 0;
            var nConf    = 0;
            var nWait    = 0;
            var nToConf  = 0;
            foreach (var r in res)
            {
                var s = st.GetValueOrDefault(r.ReservationStatusId) ?? string.Empty;
                if (s == "CREATED") nCreated++;
                if (s == "CONFIRMED") nConf++;
                if (s == "WAITLIST") nWait++;
                if (s == "CREATED" && r.ConfirmedAt is null && dCount.GetValueOrDefault(r.Id) > 0)
                    nToConf++;
            }

            var line = "[grey]Resumen de reservas activas:[/] " +
                      $"[yellow]CREATED {nCreated}[/]  ·  [green]CONFIRMED {nConf}[/]  ·  [aqua]EN ESPERA {nWait}[/]";
            if (nToConf > 0)
                line += "  ·  [bold gold1]Pendiente(s) de confirmar: " + nToConf + "[/] (Reservas → Mis reservas → Abrir: Reservas)";

            AnsiConsole.MarkupLine(line);
        }
        catch
        {
            // No bloquear el portal si el resumen falla
        }
    }

    private static void PressAnyKey()
    {
        AnsiConsole.WriteLine();
        ConsoleDashboard.FooterPressKey();
        Console.ReadKey(intercept: true);
    }

    private async Task RunBookingWizardAsync(CancellationToken ct)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var sp = scope.ServiceProvider;

        var reservationService = sp.GetRequiredService<IReservationService>();
        var passengerService = sp.GetRequiredService<IPassengerService>();
        var db = sp.GetRequiredService<AppDbContext>();

        var wizard = new Modules.Reservation.UI.ReservationBookingWizardUI(reservationService, passengerService, db);
        await wizard.RunAsync(ct);
    }

    private async Task RunAssignSeatAndFareFlowAsync(CancellationToken ct)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var sp = scope.ServiceProvider;
        var db = sp.GetRequiredService<AppDbContext>();
        var resSvc = sp.GetRequiredService<IReservationService>();
        var paxSvc = sp.GetRequiredService<IPassengerService>();
        var detailSvc = sp.GetRequiredService<IReservationDetailService>();
        var seatSvc = sp.GetRequiredService<IFlightSeatService>();

        var createdStatus = await db.ReservationStatuses.AsNoTracking()
            .FirstOrDefaultAsync(s => s.Name == "CREATED", ct);
        if (createdStatus is null)
        {
            AnsiConsole.WriteLine();
            ConsoleDashboard.Error("No se encontró el estado de reserva 'CREATED' en el sistema.");
            return;
        }

        var allMine = (await resSvc.GetMyReservationsAsync(ct))
            .Where(r => r.CancelledAt is null)
            .OrderByDescending(r => r.ReservationDate)
            .ToList();

        var candidatos = allMine
            .Where(r => r.ReservationStatusId == createdStatus.Id && r.ConfirmedAt is null)
            .ToList();

        var cIds = candidatos.Select(x => x.Id).ToList();
        var conDetalles = await db.ReservationDetails.AsNoTracking()
            .Where(d => cIds.Contains(d.ReservationId))
            .GroupBy(d => d.ReservationId)
            .ToDictionaryAsync(g => g.Key, g => g.Count(), ct);

        var needSeat = candidatos
            .Where(r => conDetalles.GetValueOrDefault(r.Id) == 0)
            .ToList();

        if (needSeat.Count == 0)
        {
            AnsiConsole.Clear();
            ConsoleDashboard.SubScreenTitle("Asignar asiento y tarifa", "Reservas en estado CREATED");
            if (candidatos.Count == 0)
            {
                ConsoleDashboard.Warning(
                    "No hay reservas CREATED pendientes de asiento. Si ya asignó asiento, vaya a Reservas → Mis reservas → Abrir: Reservas para confirmar.");
            }
            else
            {
                ConsoleDashboard.Info(
                    "Todas sus reservas CREATED ya tienen asiento. Siguiente: Reservas → Mis reservas → Abrir: Reservas (confirmar).");
            }
            return;
        }

        var sIds = needSeat.Select(r => r.ScheduledFlightId).Distinct().ToList();
        var flightInfo = await (
            from sf in db.ScheduledFlights.AsNoTracking()
            where sIds.Contains(sf.Id)
            join bf in db.BaseFlights.AsNoTracking() on sf.BaseFlightId equals bf.Id
            select new { sf.Id, bf.FlightCode, sf.DepartureDate }
        ).ToDictionaryAsync(x => x.Id, x => (x.FlightCode, x.DepartureDate), ct);

        AnsiConsole.Clear();
        ConsoleDashboard.SubScreenTitle("Asignar asiento y tarifa", "Elija reserva, pasajero, asiento libre y tarifa de cabina.");
        var table = ConsoleDashboard.NewDataTable();
        table.AddColumn("Código reserva");
        table.AddColumn("Vuelo (cód.)");
        table.AddColumn("Fecha vuelo");
        table.AddColumn("Fecha de la reserva");
        foreach (var r in needSeat)
        {
            var fc = flightInfo.GetValueOrDefault(r.ScheduledFlightId).FlightCode ?? "?";
            var dep = flightInfo.GetValueOrDefault(r.ScheduledFlightId).DepartureDate;
            table.AddRow(
                Markup.Escape(r.ReservationCode),
                Markup.Escape(fc),
                dep.ToString("yyyy-MM-dd"),
                r.ReservationDate.ToString("yyyy-MM-dd HH:mm"));
        }
        ConsoleDashboard.ShowTablePanel("Reservas listas para asignar asiento", table);
        AnsiConsole.WriteLine();

        var back = "« Volver sin guardar";
        var labels = needSeat
            .Select(r =>
            {
                var fc = flightInfo.GetValueOrDefault(r.ScheduledFlightId).FlightCode ?? "?";
                return $"{r.ReservationCode}  |  {fc}  |  {r.ReservationDate:yyyy-MM-dd HH:mm}  (id {r.Id})";
            })
            .Append(back)
            .ToList();

        var pick = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]Seleccione su reserva:[/]")
                .PageSize(12)
                .MoreChoicesText(ConsoleDashboard.SelectionMoreChoicesNav)
                .AddChoices(labels));
        if (pick == back) return;

        var idx = labels.IndexOf(pick);
        if (idx < 0 || idx >= needSeat.Count) return;
        var resRow = needSeat[idx];
        var scheduledFlightId = resRow.ScheduledFlightId;

        var passengers = (await paxSvc.GetAllAsync(ct)).ToList();
        if (passengers.Count == 0)
        {
            ConsoleDashboard.Error("No hay pasajeros en su cuenta. Cree uno en Avanzado → Mis datos (Pasajeros) e intente de nuevo.");
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
                    .Title("[yellow]¿Qué pasajero viaja en esta reserva?[/]")
                    .PageSize(8)
                    .AddChoices(pLabels));
            pax = passengers[pLabels.IndexOf(pPick)];
        }

        var avSeats = (await seatSvc.GetAvailableByFlightAsync(scheduledFlightId, ct)).ToList();
        if (avSeats.Count == 0)
        {
            ConsoleDashboard.Error(
                "No quedan asientos libres en el vuelo. Use Reservas → Reservar (lista de espera) o cancele desde Reservas → Mis reservas.");
            return;
        }

        var smIds = avSeats.Select(s => s.SeatMapId).Distinct().ToList();
        var seatMaps = await db.SeatMaps.AsNoTracking()
            .Where(m => smIds.Contains(m.Id))
            .ToDictionaryAsync(m => m.Id, ct);
        var cabinIds = seatMaps.Values.Select(m => m.CabinClassId).Distinct().ToList();
        var cabinNames = await db.CabinClasses.AsNoTracking()
            .Where(c => cabinIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c.Name, ct);

        var seatChoices = new List<(string Label, int FlightSeatId, int CabinClassId)>();
        foreach (var fs in avSeats)
        {
            if (!seatMaps.TryGetValue(fs.SeatMapId, out var sm)) continue;
            var cn = cabinNames.TryGetValue(sm.CabinClassId, out var cname) ? cname : "Clase";
            seatChoices.Add((
                $"Asiento {sm.SeatNumber}  ·  {cn}",
                fs.Id,
                sm.CabinClassId));
        }

        if (seatChoices.Count == 0)
        {
            ConsoleDashboard.Error("No se pudieron leer asientos libres. Revise el mapa de asientos de la aeronave.");
            return;
        }

        var backSeat = "« Volver (sin asignar)";
        var sPick = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]Elija un asiento libre en su vuelo:[/]")
                .PageSize(15)
                .AddChoices(seatChoices.Select(s => s.Label).Append(backSeat)));
        if (sPick == backSeat) return;
        var seatRow = seatChoices.First(t => t.Label == sPick);

        var fareOptions = await (
            from fcp in db.FlightCabinPrices.AsNoTracking()
            where fcp.ScheduledFlightId == scheduledFlightId && fcp.CabinClassId == seatRow.CabinClassId
            join ft in db.FareTypes.AsNoTracking() on fcp.FareTypeId equals ft.Id
            select new { fcp.FareTypeId, ft.Name, fcp.Price }
        ).ToListAsync(ct);
        if (fareOptions.Count == 0)
        {
            var cname = await db.CabinClasses.AsNoTracking()
                .Where(c => c.Id == seatRow.CabinClassId)
                .Select(c => c.Name)
                .FirstOrDefaultAsync(ct) ?? "?";
            ConsoleDashboard.Error(
                $"No hay precios de tarifa para vuelo n.º {scheduledFlightId} y cabina «{cname}». " +
                "Cierre e inicie la aplicación para rellenar precios (arranque), o pida a un admin cargar el catálogo «Precio por vuelo y cabina». " +
                "Mientras, puede probar un asiento de otra cabina si aplica.");
            return;
        }

        var fLabels = fareOptions
            .Select(f => $"{f.Name}  —  {f.Price.ToString("N2", System.Globalization.CultureInfo.InvariantCulture)}")
            .ToList();
        var fBack = "« Volver (sin asignar)";
        var fPick = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]Elija el tipo de tarifa (según su cabina):[/]")
                .AddChoices(fLabels.Append(fBack)));
        if (fPick == fBack) return;
        var fare = fareOptions[fLabels.IndexOf(fPick)];

        ConsoleDashboard.Info($"Guardando: reserva {resRow.ReservationCode}, asiento, pasajero y tarifa…");
        await detailSvc.CreateAsync(resRow.Id, pax.Id, seatRow.FlightSeatId, fare.FareTypeId, ct);
        AnsiConsole.WriteLine();
        ConsoleDashboard.Success("Asiento y tarifa guardados. Siguiente: Reservas → Mis reservas → Abrir: Reservas (confirmar).");
    }

    private async Task RunMyReservationsHubAsync(CancellationToken ct)
    {
        const string backPortal = "Volver al portal de clientes";
        const string backList = "« Volver a la lista (sin acción)";

        while (!ct.IsCancellationRequested)
        {
            AnsiConsole.Clear();
            ConsoleDashboard.SubScreenTitle("Mis reservas", "Asignar asiento, confirmar/cancelar, o reprogramar (solo reservas CONFIRMED).");
            ConsoleDashboard.NavigationHint();

            await using var scope = _scopeFactory.CreateAsyncScope();
            var sp = scope.ServiceProvider;

            var reservationService = sp.GetRequiredService<IReservationService>();
            var reprogram = sp.GetRequiredService<ReprogramarReservaUseCase>();
            var db = sp.GetRequiredService<AppDbContext>();

            var my = (await reservationService.GetMyReservationsAsync(ct)).OrderByDescending(r => r.ReservationDate).ToList();
            if (my.Count == 0)
            {
                ConsoleDashboard.Warning("No hay reservas en su cuenta.");
                return;
            }

            var rIds = my.Select(m => m.Id).ToList();
            var stIds = my.Select(m => m.ReservationStatusId).Distinct().ToList();
            var statusById = await db.ReservationStatuses.AsNoTracking()
                .Where(s => stIds.Contains(s.Id))
                .ToDictionaryAsync(s => s.Id, s => s.Name, ct);
            var detailCounts = await db.ReservationDetails.AsNoTracking()
                .Where(d => rIds.Contains(d.ReservationId))
                .GroupBy(d => d.ReservationId)
                .ToDictionaryAsync(g => g.Key, g => g.Count(), ct);

            var table = ConsoleDashboard.NewDataTable();
            table.AddColumn("Código");
            table.AddColumn("N° reserva");
            table.AddColumn("Vuelo (ID)");
            table.AddColumn("Estado");
            table.AddColumn("Asientos asignados");
            table.AddColumn("Fecha reserva");
            foreach (var r in my)
            {
                var st = statusById.GetValueOrDefault(r.ReservationStatusId) ?? r.ReservationStatusId.ToString();
                var nDet = detailCounts.GetValueOrDefault(r.Id, 0);
                table.AddRow(
                    Markup.Escape(r.ReservationCode),
                    r.Id.ToString(),
                    r.ScheduledFlightId.ToString(),
                    Markup.Escape(st),
                    nDet.ToString(),
                    r.ReservationDate.ToString("yyyy-MM-dd HH:mm"));
            }
            ConsoleDashboard.ShowTablePanel("Sus reservas", table);
            AnsiConsole.WriteLine();

            // Siguiente paso recomendado (dinámico)
            var createdNoSeat = my.Count(r =>
                r.CancelledAt is null
                && string.Equals(
                    statusById.GetValueOrDefault(r.ReservationStatusId) ?? string.Empty,
                    "CREATED",
                    StringComparison.Ordinal)
                && detailCounts.GetValueOrDefault(r.Id) == 0);

            var createdWithSeatNotConfirmed = my.Count(r =>
                r.CancelledAt is null
                && string.Equals(
                    statusById.GetValueOrDefault(r.ReservationStatusId) ?? string.Empty,
                    "CREATED",
                    StringComparison.Ordinal)
                && r.ConfirmedAt is null
                && detailCounts.GetValueOrDefault(r.Id) > 0);

            var waitlistCount = my.Count(r =>
                r.CancelledAt is null
                && string.Equals(
                    statusById.GetValueOrDefault(r.ReservationStatusId) ?? string.Empty,
                    "WAITLIST",
                    StringComparison.Ordinal));

            var confirmedCount = my.Count(r =>
                r.CancelledAt is null
                && string.Equals(
                    statusById.GetValueOrDefault(r.ReservationStatusId) ?? string.Empty,
                    "CONFIRMED",
                    StringComparison.Ordinal));

            if (createdNoSeat > 0 || createdWithSeatNotConfirmed > 0 || waitlistCount > 0)
            {
                ConsoleDashboard.MiniDivider();
                if (createdNoSeat > 0)
                    ConsoleDashboard.Info($"Siguiente recomendado: Asignar asiento y tarifa a {createdNoSeat} reserva(s) CREATED sin asiento (Reservas → Asignar asiento y tarifa).");
                if (createdWithSeatNotConfirmed > 0)
                    ConsoleDashboard.Info($"Siguiente recomendado: Confirmar {createdWithSeatNotConfirmed} reserva(s) con asiento ya asignado (Mis reservas → Abrir: Reservas).");
                if (waitlistCount > 0)
                    ConsoleDashboard.Info($"Siguiente recomendado: Revisar {waitlistCount} reserva(s) en lista de espera (Seguimiento → Lista de espera).");
                AnsiConsole.WriteLine();
            }

            var toConfirmCount = my.Count(r =>
                r.CancelledAt is null
                && string.Equals(
                    statusById.GetValueOrDefault(r.ReservationStatusId) ?? string.Empty,
                    "CREATED",
                    StringComparison.Ordinal)
                && r.ConfirmedAt is null
                && detailCounts.GetValueOrDefault(r.Id) > 0);

            var hubActions = new List<string>();
            if (toConfirmCount > 0)
            {
                hubActions.Add(
                    $"Confirmar reserva (módulo) — {toConfirmCount} pendiente(s) con asiento y sin confirmar");
            }
            hubActions.AddRange(
            [
                "Asignar asiento y tarifa (asistente guiado)…",
                "Abrir: Detalles de reserva (pantalla clásica)",
                "Abrir: Reservas (confirmar, cancelar, más opciones…)",
                "Cambiar de vuelo (reprogramar)…",
                backPortal
            ]);

            var action = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Seleccione una acción:[/]")
                    .PageSize(12)
                    .MoreChoicesText(ConsoleDashboard.SelectionMoreChoicesNav)
                    .AddChoices(hubActions));

            if (action == backPortal)
                return;

            if (toConfirmCount > 0
                && action.StartsWith("Confirmar reserva (módulo)", StringComparison.Ordinal))
            {
                await RunModuleUiAsync("reservation", "Reservas", ct);
                continue;
            }

            if (action.StartsWith("Asignar asiento y tarifa", StringComparison.Ordinal))
            {
                await RunAssignSeatAndFareFlowAsync(ct);
                continue;
            }

            if (action.StartsWith("Abrir: Detalles", StringComparison.Ordinal))
            {
                await RunModuleUiAsync("reservationdetail", "Detalles de reserva", ct);
                continue;
            }

            if (action.StartsWith("Abrir: Reservas", StringComparison.Ordinal))
            {
                await RunModuleUiAsync("reservation", "Reservas", ct);
                continue;
            }

            if (action.StartsWith("Cambiar de vuelo", StringComparison.Ordinal))
            {
                var confirmed = my
                    .Where(r => r.CancelledAt is null
                        && string.Equals(
                            statusById.GetValueOrDefault(r.ReservationStatusId) ?? string.Empty,
                            "CONFIRMED",
                            StringComparison.Ordinal))
                    .ToList();

                if (confirmed.Count == 0)
                {
                    ConsoleDashboard.Warning(
                        "Solo se puede reprogramar reservas en estado CONFIRMED. Confirme primero o revise el estado en la tabla.");
                    continue;
                }

                const string manId = "Escribir el ID a mano (vista arriba)";
                var cLabels = confirmed
                    .Select(r =>
                        $"{r.ReservationCode}  ·  vuelo n.º {r.ScheduledFlightId}  (id {r.Id})")
                    .ToList();

                int id;
                if (confirmed.Count == 1)
                {
                    var one = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("[yellow]Reprogramar su reserva CONFIRMED:[/]")
                            .PageSize(5)
                            .AddChoices(
                            [
                                cLabels[0],
                                manId,
                                backList
                            ]));

                    if (one == backList)
                        continue;
                    if (one == manId)
                    {
                        id = AnsiConsole.Prompt(
                            new TextPrompt<int>("[yellow]ID interno de la reserva (columna N° reserva arriba):[/]")
                                .Validate(n => n > 0, "Debe ser un entero positivo."));
                    }
                    else
                        id = confirmed[0].Id;
                }
                else
                {
                    var pick = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("[yellow]Elija la reserva a reprogramar (solo CONFIRMED):[/]")
                            .PageSize(12)
                            .MoreChoicesText(ConsoleDashboard.SelectionMoreChoicesNav)
                            .AddChoices(
                                cLabels.Append(manId).Append(backList)));

                    if (pick == backList)
                        continue;
                    if (pick == manId)
                    {
                        id = AnsiConsole.Prompt(
                            new TextPrompt<int>("[yellow]ID interno de la reserva (columna N° reserva arriba):[/]")
                                .Validate(n => n > 0, "Debe ser un entero positivo."));
                    }
                    else
                    {
                        var i = cLabels.IndexOf(pick);
                        if (i < 0)
                        {
                            ConsoleDashboard.Error("No se pudo leer la opción. Intente otra vez.");
                            continue;
                        }
                        id = confirmed[i].Id;
                    }
                }

                if (my.All(r => r.Id != id))
                {
                    ConsoleDashboard.Error("Ese ID no está entre sus reservas. Revise la tabla.");
                    continue;
                }

                await reprogram.ExecuteInteractiveAsync(id, ct);
                continue;
            }
        }
    }

    private async Task RunModuleUiAsync(string normalizedKey, string title, CancellationToken ct)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var modules = scope.ServiceProvider.GetServices<IModuleUI>().ToList();
        var map = new Dictionary<string, IModuleUI>(StringComparer.Ordinal);
        foreach (var m in modules)
        {
            var k = ModuleKeyNormalizer.Normalize(m.Key);
            if (string.IsNullOrEmpty(k)) continue;
            if (!map.ContainsKey(k)) map[k] = m;
        }

        var key = ModuleKeyNormalizer.Normalize(normalizedKey);
        if (!map.TryGetValue(key, out var module))
        {
            ConsoleDashboard.Error($"No se encontró el módulo '{key}'.");
            return;
        }

        AnsiConsole.Clear();
        ConsoleDashboard.SubScreenTitle(title, "Módulo clásico");
        await module.RunAsync(ct);
    }

    private static async Task<List<(int Id, string Label)>> BuildScheduledFlightPickerListAsync(
        AppDbContext db,
        CancellationToken ct,
        int take = 40)
    {
        var q = await (
            from sf in db.ScheduledFlights.AsNoTracking()
            join bf in db.BaseFlights.AsNoTracking() on sf.BaseFlightId equals bf.Id
            join rt in db.Routes.AsNoTracking() on bf.RouteId equals rt.Id
            join oa in db.Airports.AsNoTracking() on rt.OriginAirportId equals oa.AirportId
            join da in db.Airports.AsNoTracking() on rt.DestinationAirportId equals da.AirportId
            orderby sf.DepartureDate descending, sf.DepartureTime, sf.Id
            select new { sf.Id, bf.FlightCode, sf.DepartureDate, Oa = oa.IataCode, Da = da.IataCode }
        ).Take(take).ToListAsync(ct);

        return q
            .Select(x => (x.Id,
                $"n.º {x.Id}  {x.FlightCode}  {x.DepartureDate:yyyy-MM-dd}  {x.Oa}→{x.Da}"))
            .ToList();
    }

    private async Task RunWaitlistLookupAsync(CancellationToken ct)
    {
        AnsiConsole.Clear();
        ConsoleDashboard.SubScreenTitle(
            "Lista de espera",
            "Elija un vuelo de la lista o escriba su ID. Orden: prioridad y fecha de solicitud.");
        ConsoleDashboard.NavigationHint();

        var go = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]¿Qué desea hacer?[/]")
                .AddChoices("Consultar lista de espera de un vuelo", "« Volver al menú de clientes (sin consultar)"));
        if (go.StartsWith("« Volver", StringComparison.Ordinal))
            return;

        await using var scope = _scopeFactory.CreateAsyncScope();
        var sp  = scope.ServiceProvider;
        var db  = sp.GetRequiredService<AppDbContext>();
        var svc = sp.GetRequiredService<IWaitlistService>();

        var flights = await BuildScheduledFlightPickerListAsync(db, ct);
        const string manual = "Escribir ID de vuelo a mano (0 = volver al menú anterior)";
        const string back   = "« Volver (sin consultar)";

        int flightId;
        if (flights.Count > 0)
        {
            var pick = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Seleccione el vuelo programado:[/]")
                    .PageSize(14)
                    .MoreChoicesText(ConsoleDashboard.SelectionMoreChoicesNav)
                    .AddChoices(flights.Select(f => f.Label).Append(manual).Append(back)));
            if (pick == back)
                return;
            if (pick == manual)
            {
                flightId = AnsiConsole.Prompt(
                    new TextPrompt<int>("[yellow]ID vuelo programado [grey](0 = volver)[/]:[/]")
                        .Validate(
                            n => n >= 0,
                            "Escriba un entero ≥ 0 (0 = volver)."));
                if (flightId == 0) return;
            }
            else
            {
                var ix = flights.FindIndex(f => f.Label == pick);
                if (ix < 0)
                {
                    ConsoleDashboard.Error("No se pudo leer el vuelo elegido.");
                    PressAnyKey();
                    return;
                }
                flightId = flights[ix].Id;
            }
        }
        else
        {
            flightId = AnsiConsole.Prompt(
                new TextPrompt<int>("[yellow]ID vuelo programado [grey](0 = volver)[/]:[/]")
                    .Validate(n => n >= 0, "Escriba un entero ≥ 0 (0 = volver)."));
            if (flightId == 0) return;
        }

        var rows = (await svc.GetByFlightAsync(flightId, ct)).ToList();
        if (rows.Count == 0)
        {
            ConsoleDashboard.Warning("No hay registros de lista de espera para ese vuelo.");
            PressAnyKey();
            return;
        }

        var resIds  = rows.Select(x => x.ReservationId).Distinct().ToList();
        var resCode = await db.Reservations.AsNoTracking()
            .Where(r => resIds.Contains(r.Id))
            .ToDictionaryAsync(r => r.Id, r => r.ReservationCode, ct);

        var t = ConsoleDashboard.NewDataTable();
        t.AddColumn("ID lista");
        t.AddColumn("Cód. reserva");
        t.AddColumn("N° reserva");
        t.AddColumn("Pasajero");
        t.AddColumn("Estado");
        t.AddColumn("Prioridad");
        t.AddColumn("Fecha solicitud");
        string EstadoTexto(string e) => e switch
        {
            "PENDING" => "Pendiente",
            "CANCELLED" or "CANCELED" => "Cancelada",
            "PROMOTED" => "Promovida",
            "CONFIRMED" => "Confirmada",
            _ => e
        };
        foreach (var r in rows.OrderBy(x => x.Estado == "PENDING" ? 0 : 1).ThenBy(x => x.Prioridad).ThenBy(x => x.FechaSolicitud).ThenBy(x => x.Id))
        {
            var code = resCode.GetValueOrDefault(r.ReservationId) ?? "—";
            t.AddRow(
                r.Id.ToString(),
                Markup.Escape(code),
                r.ReservationId.ToString(),
                r.PassengerId.ToString(),
                Markup.Escape(EstadoTexto(r.Estado)),
                r.Prioridad.ToString(),
                r.FechaSolicitud.ToString("yyyy-MM-dd HH:mm"));
        }
        ConsoleDashboard.ShowTablePanel($"Lista de espera — vuelo n.º {flightId}", t);
        PressAnyKey();
    }

    private async Task RunReprogrammingHistoryLookupAsync(CancellationToken ct)
    {
        AnsiConsole.Clear();
        ConsoleDashboard.SubScreenTitle("Historial de reprogramaciones", "Auditoría de cambios de vuelo");
        ConsoleDashboard.NavigationHint();

        var mode = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]¿Cómo desea filtrar?[/]")
                .AddChoices(
                    "« Volver al menú de clientes (sin buscar)",
                    "Por número de reserva",
                    "Por número de vuelo (anterior o nuevo)",
                    "Ver los últimos 50 (todos)"));

        if (mode.StartsWith("« Volver", StringComparison.Ordinal))
            return;

        await using var scope = _scopeFactory.CreateAsyncScope();
        var svc = scope.ServiceProvider.GetRequiredService<IReprogrammingHistoryService>();

        IEnumerable<ReprogrammingHistoryDto> data = mode switch
        {
            "Por número de reserva" => await svc.GetByReservationAsync(
                AnsiConsole.Prompt(new TextPrompt<int>("[yellow]Número de reserva:[/]").Validate(x => x > 0, "Escriba un ID válido.")),
                ct),
            "Por número de vuelo (anterior o nuevo)" => await svc.GetByFlightAsync(
                AnsiConsole.Prompt(new TextPrompt<int>("[yellow]Número de vuelo programado:[/]").Validate(x => x > 0, "Escriba un ID válido.")),
                ct),
            _ => await svc.GetAllAsync(ct)
        };

        var rows = data.ToList();
        if (rows.Count == 0)
        {
            ConsoleDashboard.Warning("No hay registros para el filtro seleccionado.");
            PressAnyKey();
            return;
        }

        var db    = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var sfIds = rows.SelectMany(r => new[] { r.VueloAnteriorId, r.NuevoVueloId }).Distinct().ToList();
        var sfLabel = await (
            from sf in db.ScheduledFlights.AsNoTracking()
            where sfIds.Contains(sf.Id)
            join bf in db.BaseFlights.AsNoTracking() on sf.BaseFlightId equals bf.Id
            select new { sf.Id, bf.FlightCode, sf.DepartureDate }
        ).ToDictionaryAsync(x => x.Id, x => $"{x.FlightCode} {x.DepartureDate:yyyy-MM-dd}", ct);

        string Sf(int id) =>
            sfLabel.TryGetValue(id, out var s) ? $"{s} (#{id})" : id.ToString();

        var t = ConsoleDashboard.NewDataTable();
        t.AddColumn("ID");
        t.AddColumn("N° reserva");
        t.AddColumn("Vuelo ant.");
        t.AddColumn("Vuelo nuevo");
        t.AddColumn("Fecha cambio");
        t.AddColumn("Motivo");
        foreach (var r in rows.Take(50))
        {
            t.AddRow(
                r.Id.ToString(),
                r.ReservationId.ToString(),
                Markup.Escape(Sf(r.VueloAnteriorId)),
                Markup.Escape(Sf(r.NuevoVueloId)),
                r.FechaCambio.ToString("yyyy-MM-dd HH:mm"),
                Markup.Escape(r.Motivo ?? string.Empty));
        }
        ConsoleDashboard.ShowTablePanel("Historial de movimientos", t);
        PressAnyKey();
    }

    private static void ShowHelp()
    {
        AnsiConsole.Clear();
        ConsoleDashboard.SubScreenTitle("Ayuda — Portal de clientes", "Estructura del menú por categorías y flujo recomendado");
        var grid = new Grid().AddColumn().AddColumn();
        grid.AddRow("[aqua]Reservas[/]", "Flujo recomendado: [bold]Reservar → Asignar asiento y tarifa → Mis reservas → Abrir: Reservas → Confirmar[/].");
        grid.AddRow("[aqua]Seguimiento[/]", "Consultar [bold]lista de espera[/] por vuelo e [bold]historial[/] de reprogramaciones.");
        grid.AddRow("[aqua]Ayuda y cuenta[/]", "Guía rápida (esta pantalla) y acciones de sesión (cerrar sesión).");
        grid.AddRow("[aqua]Avanzado[/]", "Módulos avanzados (catálogos y acciones). Algunos títulos pueden estar en inglés.");
        grid.AddRow("[aqua]Tip[/]", "Si ve un ID (reserva/vuelo), es un identificador interno para consultas y soporte.");
        AnsiConsole.Write(
            new Panel(grid)
                .Header("[seagreen1 bold]  Guía  [/]")
                .Border(BoxBorder.Double)
                .BorderStyle(Style.Parse("grey"))
                .Expand());
        PressAnyKey();
    }

    private async Task RunLegacyAdvancedAsync(CancellationToken ct)
    {
        AnsiConsole.Clear();
        ConsoleDashboard.SubScreenTitle(
            "Más opciones (vista avanzada)",
            "Listas tipo CRUD; nombres en inglés en algunos módulos. Use « Volver con frecuencia.");
        ConsoleDashboard.Info("Sugerencia: use Reservas / Seguimiento / Ayuda para el flujo guiado.");
        ConsoleDashboard.NavigationHint();

        var sections = PortalAccess.ClientSections;
        if (sections.Count == 0) return;

        var pick = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]Seleccione un área:[/]")
                .PageSize(12)
                .MoreChoicesText(ConsoleDashboard.SelectionMoreChoicesNav)
                .AddChoices(sections.Select(s => s.Title).Append("« Volver al portal de clientes")));

        if (pick == "« Volver al portal de clientes")
            return;

        var section = sections.First(s => s.Title == pick);
        var moduleKeys = section.ModuleKeys;
        if (moduleKeys.Count == 0) return;

        var indexed = new List<(string ModKey, string Label)>(moduleKeys.Count);
        for (var i = 0; i < moduleKeys.Count; i++)
        {
            var nk   = ModuleKeyNormalizer.Normalize(moduleKeys[i]);
            var disp = FunctionalNavigation.GetModuleTitle(nk);
            indexed.Add((nk, $"{i + 1,2:00} — {disp}"));
        }

        var mPick = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]¿Qué módulo abrir?[/]")
                .PageSize(12)
                .MoreChoicesText(ConsoleDashboard.SelectionMoreChoicesNav)
                .AddChoices(indexed.Select(t => t.Label).Append("« Volver")));

        if (mPick == "« Volver")
            return;

        var found = indexed.Find(t => t.Label == mPick);
        if (string.IsNullOrEmpty(found.ModKey))
        {
            ConsoleDashboard.Error("No se pudo abrir el módulo elegido.");
            return;
        }

        var titleOnly = mPick;
        var sep         = mPick.IndexOf("—", StringComparison.Ordinal);
        if (sep >= 0 && sep + 1 < mPick.Length)
            titleOnly = mPick[(sep + 1)..].TrimStart();

        await RunModuleUiAsync(found.ModKey, titleOnly, ct);
    }
}
