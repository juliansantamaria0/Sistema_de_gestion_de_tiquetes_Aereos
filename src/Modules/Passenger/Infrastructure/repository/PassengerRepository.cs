namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class PassengerRepository : IPassengerRepository
{
    private readonly AppDbContext _context;

    public PassengerRepository(AppDbContext context)
    {
        _context = context;
    }

    private static PassengerAggregate ToDomain(PassengerEntity entity)
        => new(
            new PassengerId(entity.Id),
            entity.PersonId,
            entity.FrequentFlyerNumber,
            entity.NationalityId,
            entity.CreatedAt,
            entity.UpdatedAt);

    public async Task<PassengerAggregate?> GetByIdAsync(
        PassengerId       id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Passengers
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IEnumerable<PassengerAggregate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.Passengers
            .AsNoTracking()
            .OrderBy(e => e.PersonId)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task<PassengerAggregate?> GetByPersonAsync(
        int               personId,
        CancellationToken cancellationToken = default)
    {
        
        var entity = await _context.Passengers
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.PersonId == personId, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task AddAsync(
        PassengerAggregate passenger,
        CancellationToken  cancellationToken = default)
    {
        var entity = new PassengerEntity
        {
            PersonId            = passenger.PersonId,
            FrequentFlyerNumber = passenger.FrequentFlyerNumber,
            NationalityId       = passenger.NationalityId,
            CreatedAt           = passenger.CreatedAt,
            UpdatedAt           = passenger.UpdatedAt
        };
        await _context.Passengers.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(
        PassengerAggregate passenger,
        CancellationToken  cancellationToken = default)
    {
        var entity = await _context.Passengers
            .FirstOrDefaultAsync(e => e.Id == passenger.Id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"PassengerEntity with id {passenger.Id.Value} not found.");

        
        entity.FrequentFlyerNumber = passenger.FrequentFlyerNumber;
        entity.NationalityId       = passenger.NationalityId;
        entity.UpdatedAt           = passenger.UpdatedAt;

        _context.Passengers.Update(entity);
    }

    public async Task DeleteAsync(
        PassengerId       id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Passengers
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"PassengerEntity with id {id.Value} not found.");

        _context.Passengers.Remove(entity);
    }
}
