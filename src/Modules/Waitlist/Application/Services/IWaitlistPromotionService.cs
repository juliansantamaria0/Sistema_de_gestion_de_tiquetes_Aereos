using MySqlConnector;

namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Waitlist.Application.Services;

/// <summary>
/// Promueve reservas en <c>lista_espera</c> a confirmadas cuando hay asientos libres,
/// de forma atómica dentro de la <see cref="MySqlTransaction"/> indicada.
/// </summary>
public interface IWaitlistPromotionService
{
    /// <param name="onPromoted">Invocado con el código de reserva cuando una fila pasa a confirmada; puede ser null (sin salida en consola).</param>
    Task PromotePendingReservationsForFlightAsync(
        MySqlConnection connection,
        MySqlTransaction transaction,
        int scheduledFlightId,
        Action<string>? onPromoted,
        CancellationToken cancellationToken = default);
}
