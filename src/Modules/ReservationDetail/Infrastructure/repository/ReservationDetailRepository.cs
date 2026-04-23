namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class ReservationDetailRepository : IReservationDetailRepository
{
    private readonly AppDbContext _context;

    public ReservationDetailRepository(AppDbContext context)
    {
        _context = context;
    }

    

    private static ReservationDetailAggregate ToDomain(ReservationDetailEntity entity)
        => new(
            new ReservationDetailId(entity.Id),
            entity.ReservationId,
            entity.PassengerId,
            entity.FlightSeatId,
            entity.FareTypeId,
            entity.CreatedAt,
            entity.UpdatedAt);

    

    public async Task<ReservationDetailAggregate?> GetByIdAsync(
        ReservationDetailId id,
        CancellationToken   cancellationToken = default)
    {
        var entity = await _context.ReservationDetails
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IEnumerable<ReservationDetailAggregate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.ReservationDetails
            .AsNoTracking()
            .OrderBy(e => e.ReservationId)
            .ThenBy(e => e.PassengerId)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task<IEnumerable<ReservationDetailAggregate>> GetByReservationAsync(
        int               reservationId,
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.ReservationDetails
            .AsNoTracking()
            .Where(e => e.ReservationId == reservationId)
            .OrderBy(e => e.PassengerId)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task AddAsync(
        ReservationDetailAggregate reservationDetail,
        CancellationToken          cancellationToken = default)
    {
        var entity = new ReservationDetailEntity
        {
            ReservationId = reservationDetail.ReservationId,
            PassengerId   = reservationDetail.PassengerId,
            FlightSeatId  = reservationDetail.FlightSeatId,
            FareTypeId    = reservationDetail.FareTypeId,
            CreatedAt     = reservationDetail.CreatedAt,
            UpdatedAt     = reservationDetail.UpdatedAt
        };
        await _context.ReservationDetails.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(
        ReservationDetailAggregate reservationDetail,
        CancellationToken          cancellationToken = default)
    {
        var entity = await _context.ReservationDetails
            .FirstOrDefaultAsync(e => e.Id == reservationDetail.Id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"ReservationDetailEntity with id {reservationDetail.Id.Value} not found.");

        
        
        entity.FareTypeId = reservationDetail.FareTypeId;
        entity.UpdatedAt  = reservationDetail.UpdatedAt;

        _context.ReservationDetails.Update(entity);
    }

    public async Task DeleteAsync(
        ReservationDetailId id,
        CancellationToken   cancellationToken = default)
    {
        var entity = await _context.ReservationDetails
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"ReservationDetailEntity with id {id.Value} not found.");

        _context.ReservationDetails.Remove(entity);
    }
}
