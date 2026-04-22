using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

namespace Sistema_de_gestion_de_tiquetes_Aereos.Shared.Extensions;

public static class StatusHistoryExtensions
{
    public static Task AddReservationStatusHistoryAsync(
        this AppDbContext context,
        int reservationId,
        int reservationStatusId,
        string? notes,
        DateTime? changedAt = null,
        CancellationToken cancellationToken = default)
    {
        var entity = new ReservationStatusHistoryEntity
        {
            ReservationId = reservationId,
            ReservationStatusId = reservationStatusId,
            ChangedAt = changedAt ?? DateTime.UtcNow,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };

        return context.ReservationStatusHistories.AddAsync(entity, cancellationToken).AsTask();
    }

    public static Task AddTicketStatusHistoryAsync(
        this AppDbContext context,
        int ticketId,
        int ticketStatusId,
        string? notes,
        DateTime? changedAt = null,
        CancellationToken cancellationToken = default)
    {
        var entity = new TicketStatusHistoryEntity
        {
            TicketId = ticketId,
            TicketStatusId = ticketStatusId,
            ChangedAt = changedAt ?? DateTime.UtcNow,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };

        return context.TicketStatusHistories.AddAsync(entity, cancellationToken).AsTask();
    }
}
