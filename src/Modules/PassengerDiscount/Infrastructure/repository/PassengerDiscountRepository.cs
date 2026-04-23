namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class PassengerDiscountRepository : IPassengerDiscountRepository
{
    private readonly AppDbContext _context;

    public PassengerDiscountRepository(AppDbContext context)
    {
        _context = context;
    }

    

    private static PassengerDiscountAggregate ToDomain(PassengerDiscountEntity entity)
        => new(
            new PassengerDiscountId(entity.Id),
            entity.ReservationDetailId,
            entity.DiscountTypeId,
            entity.AmountApplied);

    

    public async Task<PassengerDiscountAggregate?> GetByIdAsync(
        PassengerDiscountId id,
        CancellationToken   cancellationToken = default)
    {
        var entity = await _context.PassengerDiscounts
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IEnumerable<PassengerDiscountAggregate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.PassengerDiscounts
            .AsNoTracking()
            .OrderBy(e => e.ReservationDetailId)
            .ThenBy(e => e.DiscountTypeId)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task<IEnumerable<PassengerDiscountAggregate>> GetByReservationDetailAsync(
        int               reservationDetailId,
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.PassengerDiscounts
            .AsNoTracking()
            .Where(e => e.ReservationDetailId == reservationDetailId)
            .OrderBy(e => e.DiscountTypeId)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task AddAsync(
        PassengerDiscountAggregate passengerDiscount,
        CancellationToken          cancellationToken = default)
    {
        var entity = new PassengerDiscountEntity
        {
            ReservationDetailId = passengerDiscount.ReservationDetailId,
            DiscountTypeId      = passengerDiscount.DiscountTypeId,
            AmountApplied       = passengerDiscount.AmountApplied
        };
        await _context.PassengerDiscounts.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(
        PassengerDiscountAggregate passengerDiscount,
        CancellationToken          cancellationToken = default)
    {
        var entity = await _context.PassengerDiscounts
            .FirstOrDefaultAsync(e => e.Id == passengerDiscount.Id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"PassengerDiscountEntity with id {passengerDiscount.Id.Value} not found.");

        
        
        entity.AmountApplied = passengerDiscount.AmountApplied;

        _context.PassengerDiscounts.Update(entity);
    }

    public async Task DeleteAsync(
        PassengerDiscountId id,
        CancellationToken   cancellationToken = default)
    {
        var entity = await _context.PassengerDiscounts
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"PassengerDiscountEntity with id {id.Value} not found.");

        _context.PassengerDiscounts.Remove(entity);
    }
}
