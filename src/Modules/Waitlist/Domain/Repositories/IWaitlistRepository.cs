namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Waitlist.Domain.Repositories;

using MySqlConnector;

public interface IWaitlistRepository
{
    Task<bool> ExistsPendingAsync(
        MySqlConnection connection,
        MySqlTransaction tx,
        int reservationId,
        int scheduledFlightId,
        int passengerId,
        CancellationToken ct);

    /// <summary>True si el pasajero ya tiene un cupo PENDING en el vuelo (cualquier reserva).</summary>
    Task<bool> HasPassengerPendingOnFlightAsync(
        MySqlConnection connection,
        MySqlTransaction tx,
        int scheduledFlightId,
        int passengerId,
        CancellationToken ct);

    Task<int> GetNextPriorityAsync(MySqlConnection connection, MySqlTransaction tx, int scheduledFlightId, CancellationToken ct);

    Task<int> InsertPendingAsync(
        MySqlConnection connection,
        MySqlTransaction tx,
        int reservationId,
        int scheduledFlightId,
        int passengerId,
        int fareTypeId,
        int prioridad,
        DateTime fechaSolicitudUtc,
        CancellationToken ct);

    Task<(int ListaEsperaId, int ReservationId, int PassengerId, int FareTypeId)?> GetNextPendingForUpdateAsync(
        MySqlConnection connection,
        MySqlTransaction tx,
        int scheduledFlightId,
        CancellationToken ct);

    Task MarkPromotedAsync(MySqlConnection connection, MySqlTransaction tx, int listaEsperaId, DateTime nowUtc, CancellationToken ct);
}

