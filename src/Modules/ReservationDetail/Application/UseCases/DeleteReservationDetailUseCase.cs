namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Application.UseCases;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Constants;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class DeleteReservationDetailUseCase
{
    private readonly IReservationDetailRepository _repository;
    private readonly IUnitOfWork                  _unitOfWork;
    private readonly AppDbContext                 _context;

    public DeleteReservationDetailUseCase(IReservationDetailRepository repository, IUnitOfWork unitOfWork, AppDbContext context)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _context    = context;
    }

    public async Task ExecuteAsync(int id, CancellationToken cancellationToken = default)
    {
        var detail = await _context.ReservationDetails
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"No se encontró el detalle de reserva con id {id}.");

        var reservation = await _context.Reservations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == detail.ReservationId, cancellationToken)
            ?? throw new InvalidOperationException($"No existe la reserva con id {detail.ReservationId}.");

        if (reservation.CancelledAt.HasValue)
            throw new InvalidOperationException("No se pueden eliminar detalles de una reserva cancelada.");

        if (reservation.ConfirmedAt.HasValue)
            throw new InvalidOperationException("No se pueden eliminar detalles de una reserva ya confirmada.");

        var hasIssuedTicket = await _context.Tickets
            .AsNoTracking()
            .AnyAsync(x => x.ReservationDetailId == id, cancellationToken);

        if (hasIssuedTicket)
            throw new InvalidOperationException("No se puede eliminar un detalle que ya tiene tiquete emitido.");

        
        var availableStatusId = await GetSeatStatusIdAsync(SeatStatusNames.Available, cancellationToken);
        var reservedStatusId  = await GetSeatStatusIdAsync(SeatStatusNames.Reserved, cancellationToken);

        var seat = await _context.FlightSeats
            .FirstOrDefaultAsync(x => x.Id == detail.FlightSeatId, cancellationToken);

        if (seat is not null && seat.SeatStatusId == reservedStatusId)
        {
            seat.SeatStatusId = availableStatusId;
            seat.UpdatedAt    = DateTime.UtcNow;
        }

        
        var strategy = _context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _context.Database.BeginTransactionAsync(cancellationToken);
            await _repository.DeleteAsync(new ReservationDetailId(id), cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
        });
    }

    private async Task<int> GetSeatStatusIdAsync(string name, CancellationToken cancellationToken)
    {
        var id = await _context.SeatStatuses
            .AsNoTracking()
            .Where(x => x.Name == name)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (id <= 0)
            throw new InvalidOperationException($"No existe el estado de asiento '{name}'.");

        return id;
    }
}
