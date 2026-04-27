namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReprogrammingHistory.Domain.Repositories;

using MySqlConnector;

public interface IReprogrammingHistoryRepository
{
    Task InsertAsync(
        MySqlConnection connection,
        MySqlTransaction tx,
        int reservationId,
        int vueloAnteriorId,
        int nuevoVueloId,
        DateTime fechaCambioUtc,
        string? motivo,
        CancellationToken ct);
}

