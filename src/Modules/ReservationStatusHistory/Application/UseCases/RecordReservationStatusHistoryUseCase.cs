namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.Application.UseCases;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Extensions;

public sealed class RecordReservationStatusHistoryUseCase
{
    private readonly AppDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public RecordReservationStatusHistoryUseCase(AppDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<ReservationStatusHistoryAggregate> ExecuteAsync(
        int reservationId,
        int reservationStatusId,
        string? notes,
        CancellationToken cancellationToken = default)
    {
        if (!await _context.Reservations.AsNoTracking().AnyAsync(x => x.Id == reservationId, cancellationToken))
            throw new InvalidOperationException($"No existe la reserva con id {reservationId}.");

        if (!await _context.ReservationStatuses.AsNoTracking().AnyAsync(x => x.Id == reservationStatusId, cancellationToken))
            throw new InvalidOperationException($"No existe el estado de reserva con id {reservationStatusId}.");

        var changedAt = DateTime.UtcNow;
        await _context.AddReservationStatusHistoryAsync(
            reservationId,
            reservationStatusId,
            notes,
            changedAt,
            cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        var entity = await _context.ReservationStatusHistories
            .AsNoTracking()
            .OrderByDescending(x => x.Id)
            .FirstAsync(x => x.ReservationId == reservationId && x.ReservationStatusId == reservationStatusId && x.ChangedAt == changedAt, cancellationToken);

        return new ReservationStatusHistoryAggregate(
            new ReservationStatusHistoryId(entity.Id),
            entity.ReservationId,
            entity.ReservationStatusId,
            entity.ChangedAt,
            entity.Notes);
    }
}
