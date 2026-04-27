namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Application.UseCases;

using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Spectre.Console;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReprogrammingHistory.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Waitlist.Application.Services;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Waitlist.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Infrastructure;

public sealed class ReprogramarReservaUseCase
{
    private readonly string _connectionString;
    private readonly IWaitlistRepository _waitlistRepository;
    private readonly IReprogrammingHistoryRepository _historyRepository;
    private readonly IWaitlistPromotionService _waitlistPromotion;

    public ReprogramarReservaUseCase(
        IConfiguration configuration,
        IWaitlistRepository waitlistRepository,
        IReprogrammingHistoryRepository historyRepository,
        IWaitlistPromotionService waitlistPromotion)
    {
        _connectionString = ConnectionStringResolver.GetRequiredMySqlConnectionString(configuration);
        _waitlistRepository = waitlistRepository;
        _historyRepository = historyRepository;
        _waitlistPromotion = waitlistPromotion;
    }

    public async Task ExecuteInteractiveAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            // Paso 1 — Reserva y vuelo actual (cliente, ruta, vuelo, fecha — Examen 4)
            var currentInfo = await GetReservationReprogramSnapshotAsync(connection, id, cancellationToken);
            if (currentInfo is null)
                throw new KeyNotFoundException($"No existe la reserva con id {id}.");

            AnsiConsole.Write(new Rule("[green]1)  Reserva y vuelo actual[/]").RuleStyle("grey"));
            AnsiConsole.MarkupLine($"Estado de la reserva: [bold]{Markup.Escape(currentInfo.ReservationStatusName)}[/]");
            AnsiConsole.MarkupLine(
                $"Cliente: [bold]{Markup.Escape(currentInfo.CustomerDisplayName)}[/]  (id cliente: {currentInfo.CustomerId})");
            if (!string.IsNullOrWhiteSpace(currentInfo.CustomerEmail))
                AnsiConsole.MarkupLine($"Correo: [grey]{Markup.Escape(currentInfo.CustomerEmail!)}[/]");
            AnsiConsole.MarkupLine(
                $"Código de reserva: [bold]{Markup.Escape(currentInfo.ReservationCode)}[/]  —  id reserva: {currentInfo.ReservationId}");
            AnsiConsole.MarkupLine(
                $"Ruta: [bold]{Markup.Escape(currentInfo.OriginIata)} → {Markup.Escape(currentInfo.DestinationIata)}[/]  (id ruta: {currentInfo.RouteId})");
            AnsiConsole.MarkupLine(
                $"Vuelo actual: [bold]{Markup.Escape(currentInfo.FlightCode)}[/]  —  n.º vuelo programado: {currentInfo.CurrentScheduledFlightId}  —  " +
                $"Fecha de salida: [bold]{currentInfo.DepartureDate:yyyy-MM-dd}[/]");
            AnsiConsole.MarkupLine(
                "[grey]Regla:[/] solo se ofrecerán vuelos con la [bold]misma ruta[/] y la [bold]misma fecha de salida[/], distintos al vuelo actual.");
            AnsiConsole.WriteLine();

            // Paso 2 — Vuelos compatibles
            AnsiConsole.Write(new Rule("[green]2)  Nuevo vuelo compatible[/]").RuleStyle("grey"));
            var compatibles = await GetCompatibleFlightsAsync(
                connection,
                routeId: currentInfo.RouteId,
                departureDate: currentInfo.DepartureDate,
                excludeScheduledFlightId: currentInfo.CurrentScheduledFlightId,
                cancellationToken);

            if (compatibles.Count == 0)
                throw new InvalidOperationException("No hay vuelos compatibles disponibles para la misma ruta y fecha.");

            var choices = compatibles
                .Select(f => new
                {
                    Label = $"{f.FlightCode}  {f.DepartureDate:yyyy-MM-dd} {f.DepartureTime:HH\\:mm}  " +
                            $"· Libres: {f.AvailableSeats}  ·  Vuelo n.º {f.ScheduledFlightId}",
                    f.ScheduledFlightId,
                    f.AvailableSeats
                })
                .ToList();

            var selected = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Seleccione el nuevo vuelo (misma ruta y fecha):[/]")
                    .PageSize(12)
                    .AddChoices(choices.Select(x => x.Label).Append("« Cancelar")));

            if (selected == "« Cancelar")
                return;

            var picked = choices.First(x => x.Label == selected);
            var nuevoScheduledFlightId = picked.ScheduledFlightId;

            // Paso 3 — Cupo y posible lista de espera
            AnsiConsole.WriteLine();
            AnsiConsole.Write(new Rule("[green]3)  Cupo y lista de espera[/]").RuleStyle("grey"));
            var seatsNeeded = await CountReservationDetailsReadOnlyAsync(connection, id, cancellationToken);
            var available  = await CountAvailableSeatsReadOnlyAsync(connection, nuevoScheduledFlightId, cancellationToken);

            var permitirListaEspera = false;
            if (seatsNeeded > 0 && available < seatsNeeded)
            {
                AnsiConsole.MarkupLine(
                    $"Asientos requeridos: [bold]{seatsNeeded}[/]. " +
                    $"Disponibles en el vuelo elegido: [bold]{available}[/].");
                permitirListaEspera = AnsiConsole.Confirm(
                    "[yellow]No hay cupo suficiente. ¿Desea entrar a lista de espera para ese vuelo?[/]\n" +
                    "[grey]Nota: al confirmar, se liberarán los asientos del vuelo actual y su reserva quedará en WAITLIST.[/]");
                if (!permitirListaEspera)
                    return;
            }
            else
            {
                AnsiConsole.MarkupLine(
                    $"Plazas de la reserva: [bold]{seatsNeeded}[/]. " +
                    $"Asientos libres en el nuevo vuelo: [bold]{available}[/] — " +
                    "[green]cupo suficiente para reasignar.[/]");
            }

            // Paso 4 — Resumen antes de la transacción
            AnsiConsole.WriteLine();
            AnsiConsole.Write(new Rule("[green]4)  Resumen (revise antes de confirmar)[/]").RuleStyle("grey"));
            var resumen = new Table().Border(TableBorder.Rounded).Expand();
            resumen.AddColumn("Campo");
            resumen.AddColumn("Valor");
            resumen.AddRow("Estado (antes de aplicar)", Markup.Escape(currentInfo.ReservationStatusName));
            resumen.AddRow("Cliente", Markup.Escape(currentInfo.CustomerDisplayName));
            resumen.AddRow("Ruta", Markup.Escape($"{currentInfo.OriginIata} → {currentInfo.DestinationIata}"));
            resumen.AddRow("Código de reserva", Markup.Escape(currentInfo.ReservationCode));
            resumen.AddRow("Vuelo de origen (actual)", Markup.Escape($"{currentInfo.FlightCode}  n.º {currentInfo.CurrentScheduledFlightId}"));
            resumen.AddRow("Nuevo vuelo", Markup.Escape(picked.Label));
            resumen.AddRow("Plazas a mover", seatsNeeded.ToString());
            if (seatsNeeded > 0 && available < seatsNeeded && permitirListaEspera)
                resumen.AddRow("Efecto previsto", "[yellow]Sin cupo: estado WAITLIST en el vuelo elegido.[/]");
            else
                resumen.AddRow("Efecto previsto", "[green]Reasignación de asientos; estado CONFIRMED.[/]");

            AnsiConsole.Write(new Panel(resumen).Header("[aqua]Confirmación requerida[/]"));
            AnsiConsole.WriteLine();

            if (!AnsiConsole.Confirm("[bold]¿Confirmar la reprogramación?[/]"))
            {
                AnsiConsole.MarkupLine("[grey]Operación no aplicada.[/]");
                return;
            }

            await ExecuteAsync(
                id,
                nuevoScheduledFlightId,
                permitirListaEsperaSiSinCupo: permitirListaEspera,
                cancellationToken);
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[green]Listo.[/] Reprogramación registrada y guardada en la base de datos.");
        }
        catch (MySqlException ex)
        {
            throw new InvalidOperationException(MySqlErrorFormatter.ToUserMessage(ex), ex);
        }
    }

    public async Task ExecuteAsync(
        int id,
        int nuevoScheduledFlightId,
        bool permitirListaEsperaSiSinCupo,
        CancellationToken cancellationToken = default)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var tx = await connection.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, cancellationToken);
        try
        {
            var confirmedStatusId = await GetReservationStatusIdAsync(connection, tx, "CONFIRMED", cancellationToken);
            var waitlistStatusId  = await GetReservationStatusIdAsync(connection, tx, "WAITLIST", cancellationToken);

            var reservation = await GetReservationForUpdateAsync(connection, tx, id, cancellationToken);
            if (reservation is null)
                throw new KeyNotFoundException($"No existe la reserva con id {id}.");

            if (reservation.Value.CancelledAt.HasValue)
                throw new InvalidOperationException("No se puede reprogramar una reserva cancelada.");

            if (reservation.Value.ReservationStatusId != confirmedStatusId)
                throw new InvalidOperationException("Solo se puede reprogramar una reserva en estado 'CONFIRMED'.");

            if (reservation.Value.ScheduledFlightId == nuevoScheduledFlightId)
                throw new InvalidOperationException("El nuevo vuelo no puede ser el mismo vuelo actual de la reserva.");

            var isCompatible = await IsCompatibleFlightAsync(
                connection, tx,
                vueloActualId: reservation.Value.ScheduledFlightId,
                vueloNuevoId: nuevoScheduledFlightId,
                cancellationToken);

            if (!isCompatible)
                throw new InvalidOperationException("El nuevo vuelo no es compatible. Debe ser la misma ruta y la misma fecha.");

            var details = await GetReservationDetailSnapshotForUpdateAsync(connection, tx, id, cancellationToken);
            if (details.Count == 0)
                throw new InvalidOperationException("No se puede reprogramar una reserva confirmada sin asientos asociados.");

            await UpdateSeatsStatusAsync(connection, tx, details.Select(d => d.FlightSeatId).ToList(), targetSeatStatusName: "AVAILABLE", cancellationToken);

            var seatsNeeded = details.Count;
            var newSeatIds = await TakeAvailableSeatsForUpdateAsync(connection, tx, nuevoScheduledFlightId, seatsNeeded, cancellationToken);

            string estadoResultante;
            if (newSeatIds.Count < seatsNeeded)
            {
                if (!permitirListaEsperaSiSinCupo)
                    throw new InvalidOperationException("Operación cancelada: no se ingresó a lista de espera.");

                await DeleteReservationDetailsAsync(connection, tx, id, cancellationToken);

                foreach (var d in details)
                {
                    var exists = await _waitlistRepository.ExistsPendingAsync(connection, tx, id, nuevoScheduledFlightId, d.PassengerId, cancellationToken);
                    if (exists)
                        throw new InvalidOperationException("La reserva ya está registrada en lista de espera para ese vuelo.");

                    var prioridad = await _waitlistRepository.GetNextPriorityAsync(connection, tx, nuevoScheduledFlightId, cancellationToken);
                    await _waitlistRepository.InsertPendingAsync(
                        connection,
                        tx,
                        reservationId: id,
                        scheduledFlightId: nuevoScheduledFlightId,
                        passengerId: d.PassengerId,
                        fareTypeId: d.FareTypeId,
                        prioridad: prioridad,
                        fechaSolicitudUtc: DateTime.UtcNow,
                        ct: cancellationToken);
                }

                await UpdateReservationFlightAndStatusAsync(
                    connection,
                    tx,
                    reservationId: id,
                    nuevoScheduledFlightId: nuevoScheduledFlightId,
                    nuevoStatusId: waitlistStatusId,
                    confirmedAt: null,
                    cancellationToken);

                estadoResultante = "WAITLIST";
            }
            else
            {
                await UpdateSeatsStatusAsync(connection, tx, newSeatIds, targetSeatStatusName: "OCCUPIED", cancellationToken);
                await ReplaceReservationDetailSeatsAsync(connection, tx, reservationId: id, newSeatIds: newSeatIds, cancellationToken);

                await UpdateReservationFlightAndStatusAsync(
                    connection,
                    tx,
                    reservationId: id,
                    nuevoScheduledFlightId: nuevoScheduledFlightId,
                    nuevoStatusId: confirmedStatusId,
                    confirmedAt: DateTime.UtcNow,
                    cancellationToken);

                estadoResultante = "CONFIRMED";
            }

            await _historyRepository.InsertAsync(
                connection,
                tx,
                reservationId: id,
                vueloAnteriorId: reservation.Value.ScheduledFlightId,
                nuevoVueloId: nuevoScheduledFlightId,
                fechaCambioUtc: DateTime.UtcNow,
                motivo: estadoResultante == "WAITLIST"
                    ? "Reprogramación solicitada (sin cupo, pasa a lista de espera)"
                    : "Reprogramación solicitada por el usuario",
                ct: cancellationToken);

            var vueloLiberado = reservation.Value.ScheduledFlightId;
            await _waitlistPromotion.PromotePendingReservationsForFlightAsync(
                connection,
                tx,
                vueloLiberado,
                code =>
                    AnsiConsole.MarkupLine(
                        $"[green]PROMOCIÓN[/] Reserva [bold]{Markup.Escape(code)}[/] promovida desde lista de espera al vuelo #{vueloLiberado}."),
                cancellationToken);

            await tx.CommitAsync(cancellationToken);
        }
        catch (MySqlException ex)
        {
            await tx.RollbackAsync(cancellationToken);
            throw new InvalidOperationException(MySqlErrorFormatter.ToUserMessage(ex), ex);
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static async Task<int> CountReservationDetailsReadOnlyAsync(
        MySqlConnection connection,
        int reservationId,
        CancellationToken ct)
    {
        await using var cmd = connection.CreateCommand();
        cmd.CommandText = """
                          SELECT COUNT(*)
                          FROM reservation_detail
                          WHERE reservation_id = @rid;
                          """;
        cmd.Parameters.AddWithValue("@rid", reservationId);
        var o = await cmd.ExecuteScalarAsync(ct);
        return o is null ? 0 : Convert.ToInt32(o);
    }

    private static async Task<int> GetSeatStatusIdReadOnlyAsync(
        MySqlConnection connection,
        string statusName,
        CancellationToken ct)
    {
        await using var cmd = connection.CreateCommand();
        cmd.CommandText = """
                          SELECT seat_status_id
                          FROM seat_status
                          WHERE name = @name
                          LIMIT 1;
                          """;
        cmd.Parameters.AddWithValue("@name", statusName);
        var o = await cmd.ExecuteScalarAsync(ct);
        var id = o is null ? 0 : Convert.ToInt32(o);
        if (id <= 0)
            throw new InvalidOperationException($"No existe el estado de asiento '{statusName}'.");
        return id;
    }

    private static async Task<int> CountAvailableSeatsReadOnlyAsync(
        MySqlConnection connection,
        int scheduledFlightId,
        CancellationToken ct)
    {
        var availableStatusId = await GetSeatStatusIdReadOnlyAsync(connection, "AVAILABLE", ct);
        await using var cmd = connection.CreateCommand();
        cmd.CommandText = """
                          SELECT COUNT(*)
                          FROM flight_seat
                          WHERE scheduled_flight_id = @sfid
                            AND seat_status_id = @available;
                          """;
        cmd.Parameters.AddWithValue("@sfid", scheduledFlightId);
        cmd.Parameters.AddWithValue("@available", availableStatusId);
        var o = await cmd.ExecuteScalarAsync(ct);
        return o is null ? 0 : Convert.ToInt32(o);
    }

    private sealed record ReservationReprogramSnapshot(
        int    ReservationId,
        string ReservationCode,
        int    CustomerId,
        int    CurrentScheduledFlightId,
        int    RouteId,
        DateOnly DepartureDate,
        string FlightCode,
        string CustomerDisplayName,
        string? CustomerEmail,
        string OriginIata,
        string DestinationIata,
        string ReservationStatusName);

    private static async Task<ReservationReprogramSnapshot?> GetReservationReprogramSnapshotAsync(
        MySqlConnection connection,
        int reservationId,
        CancellationToken ct)
    {
        await using var cmd = connection.CreateCommand();
        cmd.CommandText = """
                          SELECT
                            r.reservation_id,
                            r.reservation_code,
                            r.customer_id,
                            r.scheduled_flight_id,
                            bf.route_id,
                            sf.departure_date,
                            bf.flight_code,
                            COALESCE(
                              NULLIF(TRIM(CONCAT_WS(' ', p.first_name, p.last_name)), ''),
                              CONCAT('Cliente ', r.customer_id)
                            ) AS customer_name,
                            cu.email,
                            oa.iata_code,
                            da.iata_code,
                            rs.name
                          FROM reservation r
                          JOIN scheduled_flight sf ON sf.scheduled_flight_id = r.scheduled_flight_id
                          JOIN base_flight bf ON bf.base_flight_id = sf.base_flight_id
                          JOIN route rt ON rt.route_id = bf.route_id
                          JOIN airport oa ON oa.airport_id = rt.origin_airport_id
                          JOIN airport da ON da.airport_id = rt.destination_airport_id
                          JOIN customer cu ON cu.customer_id = r.customer_id
                          LEFT JOIN person p ON p.person_id = cu.person_id
                          JOIN reservation_status rs ON rs.reservation_status_id = r.reservation_status_id
                          WHERE r.reservation_id = @rid
                          LIMIT 1;
                          """;
        cmd.Parameters.AddWithValue("@rid", reservationId);
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct)) return null;
        return new ReservationReprogramSnapshot(
            reader.GetInt32(0),
            reader.GetString(1),
            reader.GetInt32(2),
            reader.GetInt32(3),
            reader.GetInt32(4),
            reader.GetFieldValue<DateOnly>(5),
            reader.GetString(6),
            reader.GetString(7),
            reader.IsDBNull(8) ? null : reader.GetString(8),
            reader.GetString(9),
            reader.GetString(10),
            reader.GetString(11));
    }

    private static async Task<List<(int ScheduledFlightId, string FlightCode, DateOnly DepartureDate, TimeOnly DepartureTime, int AvailableSeats)>> GetCompatibleFlightsAsync(
        MySqlConnection connection,
        int routeId,
        DateOnly departureDate,
        int excludeScheduledFlightId,
        CancellationToken ct)
    {
        await using var cmd = connection.CreateCommand();
        cmd.CommandText = """
                          SELECT
                            sf.scheduled_flight_id,
                            bf.flight_code,
                            sf.departure_date,
                            sf.departure_time,
                            SUM(CASE WHEN ss.name = 'AVAILABLE' THEN 1 ELSE 0 END) AS available_seats
                          FROM scheduled_flight sf
                          JOIN base_flight bf ON bf.base_flight_id = sf.base_flight_id
                          LEFT JOIN flight_seat fs ON fs.scheduled_flight_id = sf.scheduled_flight_id
                          LEFT JOIN seat_status ss ON ss.seat_status_id = fs.seat_status_id
                          WHERE bf.route_id = @routeId
                            AND sf.departure_date = @date
                            AND sf.scheduled_flight_id <> @excludeId
                          GROUP BY sf.scheduled_flight_id, bf.flight_code, sf.departure_date, sf.departure_time
                          ORDER BY sf.departure_time;
                          """;
        cmd.Parameters.AddWithValue("@routeId", routeId);
        cmd.Parameters.AddWithValue("@date", departureDate);
        cmd.Parameters.AddWithValue("@excludeId", excludeScheduledFlightId);

        var list = new List<(int, string, DateOnly, TimeOnly, int)>();
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            list.Add((
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetFieldValue<DateOnly>(2),
                reader.GetFieldValue<TimeOnly>(3),
                reader.IsDBNull(4) ? 0 : Convert.ToInt32(reader.GetValue(4))));
        }
        return list;
    }

    private static async Task<int> GetReservationStatusIdAsync(
        MySqlConnection connection,
        MySqlTransaction tx,
        string statusName,
        CancellationToken ct)
    {
        await using var cmd = connection.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = """
                          SELECT reservation_status_id
                          FROM reservation_status
                          WHERE name = @name
                          LIMIT 1;
                          """;
        cmd.Parameters.AddWithValue("@name", statusName);
        var result = await cmd.ExecuteScalarAsync(ct);
        var id = result is null ? 0 : Convert.ToInt32(result);
        if (id <= 0)
            throw new InvalidOperationException($"No existe el estado de reserva '{statusName}'.");
        return id;
    }

    private static async Task<(int ReservationStatusId, int ScheduledFlightId, int CustomerId, DateTime? CancelledAt)?> GetReservationForUpdateAsync(
        MySqlConnection connection,
        MySqlTransaction tx,
        int reservationId,
        CancellationToken ct)
    {
        await using var cmd = connection.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = """
                          SELECT reservation_status_id, scheduled_flight_id, customer_id, cancelled_at
                          FROM reservation
                          WHERE reservation_id = @id
                          FOR UPDATE;
                          """;
        cmd.Parameters.AddWithValue("@id", reservationId);
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct)) return null;
        return (
            ReservationStatusId: reader.GetInt32(0),
            ScheduledFlightId: reader.GetInt32(1),
            CustomerId: reader.GetInt32(2),
            CancelledAt: reader.IsDBNull(3) ? null : reader.GetDateTime(3));
    }

    private static async Task<bool> IsCompatibleFlightAsync(
        MySqlConnection connection,
        MySqlTransaction tx,
        int vueloActualId,
        int vueloNuevoId,
        CancellationToken ct)
    {
        await using var cmd = connection.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = """
                          SELECT
                            (bf1.route_id = bf2.route_id) AS same_route,
                            (sf1.departure_date = sf2.departure_date) AS same_date
                          FROM scheduled_flight sf1
                          JOIN base_flight bf1 ON bf1.base_flight_id = sf1.base_flight_id
                          JOIN scheduled_flight sf2 ON sf2.scheduled_flight_id = @newId
                          JOIN base_flight bf2 ON bf2.base_flight_id = sf2.base_flight_id
                          WHERE sf1.scheduled_flight_id = @oldId
                          LIMIT 1;
                          """;
        cmd.Parameters.AddWithValue("@oldId", vueloActualId);
        cmd.Parameters.AddWithValue("@newId", vueloNuevoId);
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct))
            throw new InvalidOperationException("No fue posible validar compatibilidad: vuelo no encontrado.");
        return reader.GetBoolean(0) && reader.GetBoolean(1);
    }

    private static async Task<List<(int ReservationDetailId, int PassengerId, int FareTypeId, int FlightSeatId)>> GetReservationDetailSnapshotForUpdateAsync(
        MySqlConnection connection,
        MySqlTransaction tx,
        int reservationId,
        CancellationToken ct)
    {
        await using var cmd = connection.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = """
                          SELECT reservation_detail_id, passenger_id, fare_type_id, flight_seat_id
                          FROM reservation_detail
                          WHERE reservation_id = @rid
                          ORDER BY reservation_detail_id
                          FOR UPDATE;
                          """;
        cmd.Parameters.AddWithValue("@rid", reservationId);

        var list = new List<(int, int, int, int)>();
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            list.Add((
                reader.GetInt32(0),
                reader.GetInt32(1),
                reader.GetInt32(2),
                reader.GetInt32(3)));
        }
        return list;
    }

    private static async Task UpdateSeatsStatusAsync(
        MySqlConnection connection,
        MySqlTransaction tx,
        IReadOnlyList<int> seatIds,
        string targetSeatStatusName,
        CancellationToken ct)
    {
        if (seatIds.Count == 0) return;

        var seatStatusId = await GetSeatStatusIdAsync(connection, tx, targetSeatStatusName, ct);

        await using var cmd = connection.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = $"""
                           UPDATE flight_seat
                           SET seat_status_id = @sid, updated_at = @now
                           WHERE flight_seat_id IN ({string.Join(", ", seatIds.Select((_, i) => $"@p{i}"))});
                           """;
        cmd.Parameters.AddWithValue("@sid", seatStatusId);
        cmd.Parameters.AddWithValue("@now", DateTime.UtcNow);
        for (var i = 0; i < seatIds.Count; i++)
            cmd.Parameters.AddWithValue($"@p{i}", seatIds[i]);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    private static async Task<int> GetSeatStatusIdAsync(
        MySqlConnection connection,
        MySqlTransaction tx,
        string statusName,
        CancellationToken ct)
    {
        await using var cmd = connection.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = """
                          SELECT seat_status_id
                          FROM seat_status
                          WHERE name = @name
                          LIMIT 1;
                          """;
        cmd.Parameters.AddWithValue("@name", statusName);
        var result = await cmd.ExecuteScalarAsync(ct);
        var id = result is null ? 0 : Convert.ToInt32(result);
        if (id <= 0)
            throw new InvalidOperationException($"No existe el estado de asiento '{statusName}'.");
        return id;
    }

    private static async Task<List<int>> TakeAvailableSeatsForUpdateAsync(
        MySqlConnection connection,
        MySqlTransaction tx,
        int scheduledFlightId,
        int seatsNeeded,
        CancellationToken ct)
    {
        if (seatsNeeded <= 0) return [];

        var availableStatusId = await GetSeatStatusIdAsync(connection, tx, "AVAILABLE", ct);

        await using var cmd = connection.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = """
                          SELECT flight_seat_id
                          FROM flight_seat
                          WHERE scheduled_flight_id = @sfid
                            AND seat_status_id = @available
                          ORDER BY flight_seat_id
                          LIMIT @limit
                          FOR UPDATE;
                          """;
        cmd.Parameters.AddWithValue("@sfid", scheduledFlightId);
        cmd.Parameters.AddWithValue("@available", availableStatusId);
        cmd.Parameters.AddWithValue("@limit", seatsNeeded);

        var list = new List<int>();
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
            list.Add(reader.GetInt32(0));
        return list;
    }

    private static async Task ReplaceReservationDetailSeatsAsync(
        MySqlConnection connection,
        MySqlTransaction tx,
        int reservationId,
        IReadOnlyList<int> newSeatIds,
        CancellationToken ct)
    {
        await using var selectCmd = connection.CreateCommand();
        selectCmd.Transaction = tx;
        selectCmd.CommandText = """
                                SELECT reservation_detail_id
                                FROM reservation_detail
                                WHERE reservation_id = @rid
                                ORDER BY reservation_detail_id
                                FOR UPDATE;
                                """;
        selectCmd.Parameters.AddWithValue("@rid", reservationId);
        var detailIds = new List<int>();
        await using (var reader = await selectCmd.ExecuteReaderAsync(ct))
        {
            while (await reader.ReadAsync(ct))
                detailIds.Add(reader.GetInt32(0));
        }

        if (detailIds.Count != newSeatIds.Count)
            throw new InvalidOperationException("No es posible reasignar asientos: detalles inconsistentes.");

        for (var i = 0; i < detailIds.Count; i++)
        {
            await using var updateCmd = connection.CreateCommand();
            updateCmd.Transaction = tx;
            updateCmd.CommandText = """
                                    UPDATE reservation_detail
                                    SET flight_seat_id = @seatId, updated_at = @now
                                    WHERE reservation_detail_id = @did;
                                    """;
            updateCmd.Parameters.AddWithValue("@seatId", newSeatIds[i]);
            updateCmd.Parameters.AddWithValue("@now", DateTime.UtcNow);
            updateCmd.Parameters.AddWithValue("@did", detailIds[i]);
            await updateCmd.ExecuteNonQueryAsync(ct);
        }
    }

    private static async Task DeleteReservationDetailsAsync(
        MySqlConnection connection,
        MySqlTransaction tx,
        int reservationId,
        CancellationToken ct)
    {
        await using var cmd = connection.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = """
                          DELETE FROM reservation_detail
                          WHERE reservation_id = @rid;
                          """;
        cmd.Parameters.AddWithValue("@rid", reservationId);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    private static async Task UpdateReservationFlightAndStatusAsync(
        MySqlConnection connection,
        MySqlTransaction tx,
        int reservationId,
        int nuevoScheduledFlightId,
        int nuevoStatusId,
        DateTime? confirmedAt,
        CancellationToken ct)
    {
        await using var cmd = connection.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = """
                          UPDATE reservation
                          SET scheduled_flight_id = @sfid,
                              reservation_status_id = @statusId,
                              confirmed_at = @confirmedAt,
                              updated_at = @now
                          WHERE reservation_id = @rid;
                          """;
        cmd.Parameters.AddWithValue("@sfid", nuevoScheduledFlightId);
        cmd.Parameters.AddWithValue("@statusId", nuevoStatusId);
        cmd.Parameters.AddWithValue("@confirmedAt", confirmedAt);
        cmd.Parameters.AddWithValue("@now", DateTime.UtcNow);
        cmd.Parameters.AddWithValue("@rid", reservationId);
        await cmd.ExecuteNonQueryAsync(ct);
    }
}
