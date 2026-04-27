namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Waitlist.Application.Services;

using System.Data;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Waitlist.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Waitlist.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Infrastructure;

public sealed class WaitlistService : IWaitlistService
{
    private readonly string _connectionString;
    private readonly IWaitlistRepository _repo;

    public WaitlistService(IConfiguration configuration, IWaitlistRepository repo)
    {
        _connectionString = ConnectionStringResolver.GetRequiredMySqlConnectionString(configuration);
        _repo = repo;
    }

    public async Task<WaitlistEntryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = """
                          SELECT lista_espera_id, reservation_id, scheduled_flight_id, passenger_id, fare_type_id,
                                 fecha_solicitud, prioridad, estado, created_at, updated_at
                          FROM lista_espera
                          WHERE lista_espera_id = @id
                          LIMIT 1;
                          """;
        cmd.Parameters.AddWithValue("@id", id);
        await using var r = await cmd.ExecuteReaderAsync(cancellationToken);
        if (!await r.ReadAsync(cancellationToken)) return null;
        return Map(r);
    }

    public async Task<IEnumerable<WaitlistEntryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = """
                          SELECT lista_espera_id, reservation_id, scheduled_flight_id, passenger_id, fare_type_id,
                                 fecha_solicitud, prioridad, estado, created_at, updated_at
                          FROM lista_espera
                          ORDER BY scheduled_flight_id, estado, prioridad, fecha_solicitud, lista_espera_id
                          LIMIT 200;
                          """;
        var list = new List<WaitlistEntryDto>();
        await using var r = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await r.ReadAsync(cancellationToken))
            list.Add(Map(r));
        return list;
    }

    public async Task<IEnumerable<WaitlistEntryDto>> GetByFlightAsync(int scheduledFlightId, CancellationToken cancellationToken = default)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = """
                          SELECT lista_espera_id, reservation_id, scheduled_flight_id, passenger_id, fare_type_id,
                                 fecha_solicitud, prioridad, estado, created_at, updated_at
                          FROM lista_espera
                          WHERE scheduled_flight_id = @sfid
                          ORDER BY estado, prioridad, fecha_solicitud, lista_espera_id;
                          """;
        cmd.Parameters.AddWithValue("@sfid", scheduledFlightId);
        var list = new List<WaitlistEntryDto>();
        await using var r = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await r.ReadAsync(cancellationToken))
            list.Add(Map(r));
        return list;
    }

    public async Task<WaitlistEntryDto> AddAsync(
        int reservationId,
        int scheduledFlightId,
        int passengerId,
        int fareTypeId,
        CancellationToken cancellationToken = default)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var tx = await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        try
        {
            if (await _repo.ExistsPendingAsync(connection, tx, reservationId, scheduledFlightId, passengerId, cancellationToken))
                throw new InvalidOperationException("Ya existe un registro PENDING para esta reserva/pasajero/vuelo.");

            var prioridad = await _repo.GetNextPriorityAsync(connection, tx, scheduledFlightId, cancellationToken);
            var now = DateTime.UtcNow;
            var id = await _repo.InsertPendingAsync(connection, tx, reservationId, scheduledFlightId, passengerId, fareTypeId, prioridad, now, cancellationToken);
            await tx.CommitAsync(cancellationToken);

            return (await GetByIdAsync(id, cancellationToken))!;
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task CancelEntryAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = """
                          UPDATE lista_espera
                          SET estado = 'CANCELLED', updated_at = @now
                          WHERE lista_espera_id = @id;
                          """;
        cmd.Parameters.AddWithValue("@now", DateTime.UtcNow);
        cmd.Parameters.AddWithValue("@id", id);
        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    private static WaitlistEntryDto Map(MySqlDataReader r)
        => new(
            r.GetInt32(0),
            r.GetInt32(1),
            r.GetInt32(2),
            r.GetInt32(3),
            r.GetInt32(4),
            r.GetDateTime(5),
            r.GetInt32(6),
            r.GetString(7),
            r.GetDateTime(8),
            r.IsDBNull(9) ? null : r.GetDateTime(9));
}

