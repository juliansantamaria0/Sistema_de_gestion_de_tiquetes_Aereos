namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReprogrammingHistory.Infrastructure.Repository;

using MySqlConnector;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReprogrammingHistory.Domain.Repositories;

public sealed class ReprogrammingHistoryRepository : IReprogrammingHistoryRepository
{
    public async Task InsertAsync(
        MySqlConnection connection,
        MySqlTransaction tx,
        int reservationId,
        int vueloAnteriorId,
        int nuevoVueloId,
        DateTime fechaCambioUtc,
        string? motivo,
        CancellationToken ct)
    {
        await using var cmd = connection.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = """
                          INSERT INTO historial_reprogramaciones
                            (reservation_id, vuelo_anterior_id, nuevo_vuelo_id, fecha_cambio, motivo, created_at)
                          VALUES
                            (@rid, @old, @new, @fecha, @motivo, @created);
                          """;
        cmd.Parameters.AddWithValue("@rid", reservationId);
        cmd.Parameters.AddWithValue("@old", vueloAnteriorId);
        cmd.Parameters.AddWithValue("@new", nuevoVueloId);
        cmd.Parameters.AddWithValue("@fecha", fechaCambioUtc);
        cmd.Parameters.AddWithValue("@motivo", motivo);
        cmd.Parameters.AddWithValue("@created", fechaCambioUtc);
        await cmd.ExecuteNonQueryAsync(ct);
    }
}

