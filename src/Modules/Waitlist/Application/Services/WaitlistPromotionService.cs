using MySqlConnector;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Waitlist.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Constants;

namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Waitlist.Application.Services;

public sealed class WaitlistPromotionService : IWaitlistPromotionService
{
    private readonly IWaitlistRepository _waitlist;

    public WaitlistPromotionService(IWaitlistRepository waitlist) => _waitlist = waitlist;

    public async Task PromotePendingReservationsForFlightAsync(
        MySqlConnection connection,
        MySqlTransaction transaction,
        int scheduledFlightId,
        Action<string>? onPromoted,
        CancellationToken cancellationToken = default)
    {
        var ct = cancellationToken;
        while (true)
        {
            var next = await _waitlist.GetNextPendingForUpdateAsync(connection, transaction, scheduledFlightId, ct);
            if (next is null) return;

            var available = await CountAvailableSeatsAsync(connection, transaction, scheduledFlightId, ct);
            if (available < 1) return;

            var newSeatIds = await TakeAvailableSeatsForUpdateAsync(connection, transaction, scheduledFlightId, 1, ct);
            if (newSeatIds.Count < 1) return;

            await UpdateSeatsStatusAsync(
                connection, transaction, newSeatIds, SeatStatusNames.Occupied, ct);

            await InsertReservationDetailAsync(
                connection,
                transaction,
                next.Value.ReservationId,
                next.Value.PassengerId,
                newSeatIds[0],
                next.Value.FareTypeId,
                ct);

            var confirmedId = await GetReservationStatusIdAsync(connection, transaction, "CONFIRMED", ct);
            await UpdateReservationFlightAndStatusAsync(
                connection,
                transaction,
                next.Value.ReservationId,
                scheduledFlightId,
                confirmedId,
                DateTime.UtcNow,
                ct);

            await _waitlist.MarkPromotedAsync(
                connection, transaction, next.Value.ListaEsperaId, DateTime.UtcNow, ct);

            var code = await GetReservationCodeAsync(connection, transaction, next.Value.ReservationId, ct);
            onPromoted?.Invoke(code);
        }
    }

    private static async Task<int> CountAvailableSeatsAsync(
        MySqlConnection connection,
        MySqlTransaction transaction,
        int scheduledFlightId,
        CancellationToken ct)
    {
        var availableId = await GetSeatStatusIdAsync(connection, transaction, SeatStatusNames.Available, ct);
        await using var cmd = connection.CreateCommand();
        cmd.Transaction = transaction;
        cmd.CommandText = """
                            SELECT COUNT(*)
                            FROM flight_seat
                            WHERE scheduled_flight_id = @sfid
                              AND seat_status_id = @available;
                            """;
        cmd.Parameters.AddWithValue("@sfid", scheduledFlightId);
        cmd.Parameters.AddWithValue("@available", availableId);
        var o = await cmd.ExecuteScalarAsync(ct);
        return o is null ? 0 : Convert.ToInt32(o);
    }

    private static async Task<List<int>> TakeAvailableSeatsForUpdateAsync(
        MySqlConnection connection,
        MySqlTransaction transaction,
        int scheduledFlightId,
        int seatsNeeded,
        CancellationToken ct)
    {
        if (seatsNeeded <= 0) return [];

        var availableId = await GetSeatStatusIdAsync(connection, transaction, SeatStatusNames.Available, ct);
        await using var cmd = connection.CreateCommand();
        cmd.Transaction = transaction;
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
        cmd.Parameters.AddWithValue("@available", availableId);
        cmd.Parameters.AddWithValue("@limit", seatsNeeded);
        var list = new List<int>();
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct)) list.Add(reader.GetInt32(0));
        return list;
    }

    private static async Task UpdateSeatsStatusAsync(
        MySqlConnection connection,
        MySqlTransaction transaction,
        IReadOnlyList<int> seatIds,
        string targetSeatStatusName,
        CancellationToken ct)
    {
        if (seatIds.Count == 0) return;
        var seatStatusId = await GetSeatStatusIdAsync(connection, transaction, targetSeatStatusName, ct);
        await using var cmd = connection.CreateCommand();
        cmd.Transaction = transaction;
        cmd.CommandText = $"""
                             UPDATE flight_seat
                             SET seat_status_id = @sid, updated_at = @now
                             WHERE flight_seat_id IN ({string.Join(", ", seatIds.Select((_, i) => $"@p{i}"))});
                             """;
        cmd.Parameters.AddWithValue("@sid", seatStatusId);
        cmd.Parameters.AddWithValue("@now", DateTime.UtcNow);
        for (var i = 0; i < seatIds.Count; i++) cmd.Parameters.AddWithValue($"@p{i}", seatIds[i]);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    private static async Task<int> GetSeatStatusIdAsync(
        MySqlConnection connection,
        MySqlTransaction transaction,
        string name,
        CancellationToken ct)
    {
        await using var cmd = connection.CreateCommand();
        cmd.Transaction = transaction;
        cmd.CommandText = """
                            SELECT seat_status_id
                            FROM seat_status
                            WHERE name = @name
                            LIMIT 1;
                            """;
        cmd.Parameters.AddWithValue("@name", name);
        var o = await cmd.ExecuteScalarAsync(ct);
        var id = o is null ? 0 : Convert.ToInt32(o);
        if (id <= 0) throw new InvalidOperationException($"No existe el estado de asiento '{name}'.");
        return id;
    }

    private static async Task<int> GetReservationStatusIdAsync(
        MySqlConnection connection,
        MySqlTransaction transaction,
        string name,
        CancellationToken ct)
    {
        await using var cmd = connection.CreateCommand();
        cmd.Transaction = transaction;
        cmd.CommandText = """
                            SELECT reservation_status_id
                            FROM reservation_status
                            WHERE name = @name
                            LIMIT 1;
                            """;
        cmd.Parameters.AddWithValue("@name", name);
        var o = await cmd.ExecuteScalarAsync(ct);
        var id = o is null ? 0 : Convert.ToInt32(o);
        if (id <= 0) throw new InvalidOperationException($"No existe el estado de reserva '{name}'.");
        return id;
    }

    private static async Task UpdateReservationFlightAndStatusAsync(
        MySqlConnection connection,
        MySqlTransaction transaction,
        int reservationId,
        int scheduledFlightId,
        int statusId,
        DateTime? confirmedAt,
        CancellationToken ct)
    {
        await using var cmd = connection.CreateCommand();
        cmd.Transaction = transaction;
        cmd.CommandText = """
                            UPDATE reservation
                            SET scheduled_flight_id = @sfid,
                                reservation_status_id = @statusId,
                                confirmed_at = @confirmedAt,
                                updated_at = @now
                            WHERE reservation_id = @rid;
                            """;
        cmd.Parameters.AddWithValue("@sfid", scheduledFlightId);
        cmd.Parameters.AddWithValue("@statusId", statusId);
        cmd.Parameters.AddWithValue("@confirmedAt", confirmedAt);
        cmd.Parameters.AddWithValue("@now", DateTime.UtcNow);
        cmd.Parameters.AddWithValue("@rid", reservationId);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    private static async Task InsertReservationDetailAsync(
        MySqlConnection connection,
        MySqlTransaction transaction,
        int reservationId,
        int passengerId,
        int flightSeatId,
        int fareTypeId,
        CancellationToken ct)
    {
        await using var cmd = connection.CreateCommand();
        cmd.Transaction = transaction;
        cmd.CommandText = """
                            INSERT INTO reservation_detail
                              (reservation_id, passenger_id, flight_seat_id, fare_type_id, created_at, updated_at)
                            VALUES
                              (@rid, @pid, @seat, @fare, @now, NULL);
                            """;
        cmd.Parameters.AddWithValue("@rid", reservationId);
        cmd.Parameters.AddWithValue("@pid", passengerId);
        cmd.Parameters.AddWithValue("@seat", flightSeatId);
        cmd.Parameters.AddWithValue("@fare", fareTypeId);
        cmd.Parameters.AddWithValue("@now", DateTime.UtcNow);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    private static async Task<string> GetReservationCodeAsync(
        MySqlConnection connection,
        MySqlTransaction transaction,
        int reservationId,
        CancellationToken ct)
    {
        await using var cmd = connection.CreateCommand();
        cmd.Transaction = transaction;
        cmd.CommandText = """
                            SELECT reservation_code
                            FROM reservation
                            WHERE reservation_id = @id
                            LIMIT 1;
                            """;
        cmd.Parameters.AddWithValue("@id", reservationId);
        var o = await cmd.ExecuteScalarAsync(ct);
        return Convert.ToString(o) ?? reservationId.ToString();
    }
}
