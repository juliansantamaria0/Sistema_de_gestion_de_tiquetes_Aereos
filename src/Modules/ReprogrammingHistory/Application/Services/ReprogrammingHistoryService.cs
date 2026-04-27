namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReprogrammingHistory.Application.Services;

using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReprogrammingHistory.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Infrastructure;

public sealed class ReprogrammingHistoryService : IReprogrammingHistoryService
{
    private readonly string _connectionString;

    public ReprogrammingHistoryService(IConfiguration configuration)
    {
        _connectionString = ConnectionStringResolver.GetRequiredMySqlConnectionString(configuration);
    }

    public async Task<IEnumerable<ReprogrammingHistoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        return await QueryAsync(connection, "1=1", null, cancellationToken);
    }

    public async Task<IEnumerable<ReprogrammingHistoryDto>> GetByReservationAsync(int reservationId, CancellationToken cancellationToken = default)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        return await QueryAsync(connection, "reservation_id = @id", reservationId, cancellationToken);
    }

    public async Task<IEnumerable<ReprogrammingHistoryDto>> GetByFlightAsync(int scheduledFlightId, CancellationToken cancellationToken = default)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        return await QueryAsync(connection, "(vuelo_anterior_id = @id OR nuevo_vuelo_id = @id)", scheduledFlightId, cancellationToken);
    }

    private static async Task<IEnumerable<ReprogrammingHistoryDto>> QueryAsync(
        MySqlConnection connection,
        string where,
        int? id,
        CancellationToken ct)
    {
        await using var cmd = connection.CreateCommand();
        cmd.CommandText = $"""
                           SELECT historial_reprogramacion_id, reservation_id, vuelo_anterior_id, nuevo_vuelo_id, fecha_cambio, motivo, created_at
                           FROM historial_reprogramaciones
                           WHERE {where}
                           ORDER BY fecha_cambio DESC
                           LIMIT 200;
                           """;
        if (id.HasValue) cmd.Parameters.AddWithValue("@id", id.Value);

        var list = new List<ReprogrammingHistoryDto>();
        await using var r = await cmd.ExecuteReaderAsync(ct);
        while (await r.ReadAsync(ct))
        {
            list.Add(new ReprogrammingHistoryDto(
                r.GetInt32(0),
                r.GetInt32(1),
                r.GetInt32(2),
                r.GetInt32(3),
                r.GetDateTime(4),
                r.IsDBNull(5) ? null : r.GetString(5),
                r.GetDateTime(6)));
        }
        return list;
    }
}

