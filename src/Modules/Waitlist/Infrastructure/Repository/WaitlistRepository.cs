namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Waitlist.Infrastructure.Repository;

using MySqlConnector;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Waitlist.Domain.Repositories;

public sealed class WaitlistRepository : IWaitlistRepository
{
    public async Task<bool> ExistsPendingAsync(
        MySqlConnection connection,
        MySqlTransaction tx,
        int reservationId,
        int scheduledFlightId,
        int passengerId,
        CancellationToken ct)
    {
        await using var cmd = connection.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = """
                          SELECT 1
                          FROM lista_espera
                          WHERE reservation_id = @rid
                            AND scheduled_flight_id = @sfid
                            AND passenger_id = @pid
                            AND estado = 'PENDING'
                          LIMIT 1;
                          """;
        cmd.Parameters.AddWithValue("@rid", reservationId);
        cmd.Parameters.AddWithValue("@sfid", scheduledFlightId);
        cmd.Parameters.AddWithValue("@pid", passengerId);
        return (await cmd.ExecuteScalarAsync(ct)) is not null;
    }

    public async Task<bool> HasPassengerPendingOnFlightAsync(
        MySqlConnection connection,
        MySqlTransaction tx,
        int scheduledFlightId,
        int passengerId,
        CancellationToken ct)
    {
        await using var cmd = connection.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = """
                          SELECT 1
                          FROM lista_espera
                          WHERE scheduled_flight_id = @sfid
                            AND passenger_id = @pid
                            AND estado = 'PENDING'
                          LIMIT 1;
                          """;
        cmd.Parameters.AddWithValue("@sfid", scheduledFlightId);
        cmd.Parameters.AddWithValue("@pid", passengerId);
        return (await cmd.ExecuteScalarAsync(ct)) is not null;
    }

    public async Task<int> GetNextPriorityAsync(MySqlConnection connection, MySqlTransaction tx, int scheduledFlightId, CancellationToken ct)
    {
        await using var cmd = connection.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = """
                          SELECT COALESCE(MAX(prioridad), 0) + 1
                          FROM lista_espera
                          WHERE scheduled_flight_id = @sfid;
                          """;
        cmd.Parameters.AddWithValue("@sfid", scheduledFlightId);
        var result = await cmd.ExecuteScalarAsync(ct);
        return result is null ? 1 : Convert.ToInt32(result);
    }

    public async Task<int> InsertPendingAsync(
        MySqlConnection connection,
        MySqlTransaction tx,
        int reservationId,
        int scheduledFlightId,
        int passengerId,
        int fareTypeId,
        int prioridad,
        DateTime fechaSolicitudUtc,
        CancellationToken ct)
    {
        await using var cmd = connection.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = """
                          INSERT INTO lista_espera
                            (reservation_id, scheduled_flight_id, passenger_id, fare_type_id,
                             fecha_solicitud, prioridad, estado, created_at, updated_at)
                          VALUES
                            (@rid, @sfid, @pid, @fare, @fecha, @prioridad, 'PENDING', @created, NULL);
                          """;
        cmd.Parameters.AddWithValue("@rid", reservationId);
        cmd.Parameters.AddWithValue("@sfid", scheduledFlightId);
        cmd.Parameters.AddWithValue("@pid", passengerId);
        cmd.Parameters.AddWithValue("@fare", fareTypeId);
        cmd.Parameters.AddWithValue("@fecha", fechaSolicitudUtc);
        cmd.Parameters.AddWithValue("@prioridad", prioridad);
        cmd.Parameters.AddWithValue("@created", fechaSolicitudUtc);
        await cmd.ExecuteNonQueryAsync(ct);
        return Convert.ToInt32(cmd.LastInsertedId);
    }

    public async Task<(int ListaEsperaId, int ReservationId, int PassengerId, int FareTypeId)?> GetNextPendingForUpdateAsync(
        MySqlConnection connection,
        MySqlTransaction tx,
        int scheduledFlightId,
        CancellationToken ct)
    {
        await using var cmd = connection.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = """
                          SELECT lista_espera_id, reservation_id, passenger_id, fare_type_id
                          FROM lista_espera
                          WHERE scheduled_flight_id = @sfid
                            AND estado = 'PENDING'
                          ORDER BY prioridad, fecha_solicitud, lista_espera_id
                          LIMIT 1
                          FOR UPDATE;
                          """;
        cmd.Parameters.AddWithValue("@sfid", scheduledFlightId);
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct)) return null;
        return (reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3));
    }

    public async Task MarkPromotedAsync(MySqlConnection connection, MySqlTransaction tx, int listaEsperaId, DateTime nowUtc, CancellationToken ct)
    {
        // Compatibilidad: si la base ya tenía lista_espera sin la columna fecha_promocion,
        // no fallamos la promoción (Examen 4). Simplemente marcamos PROMOTED + updated_at.
        try
        {
            await using var cmd = connection.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = """
                              UPDATE lista_espera
                              SET estado = 'PROMOTED', fecha_promocion = @now, updated_at = @now
                              WHERE lista_espera_id = @id;
                              """;
            cmd.Parameters.AddWithValue("@now", nowUtc);
            cmd.Parameters.AddWithValue("@id", listaEsperaId);
            await cmd.ExecuteNonQueryAsync(ct);
        }
        catch (MySqlException ex) when (ex.Number == 1054) // Unknown column
        {
            await using var cmd = connection.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = """
                              UPDATE lista_espera
                              SET estado = 'PROMOTED', updated_at = @now
                              WHERE lista_espera_id = @id;
                              """;
            cmd.Parameters.AddWithValue("@now", nowUtc);
            cmd.Parameters.AddWithValue("@id", listaEsperaId);
            await cmd.ExecuteNonQueryAsync(ct);
        }
    }
}

