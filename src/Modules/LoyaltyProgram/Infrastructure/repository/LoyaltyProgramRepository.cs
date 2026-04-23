namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class LoyaltyProgramRepository : ILoyaltyProgramRepository
{
    private readonly AppDbContext _context;

    public LoyaltyProgramRepository(AppDbContext context)
    {
        _context = context;
    }

    private static LoyaltyProgramAggregate ToDomain(LoyaltyProgramEntity entity)
        => new(new LoyaltyProgramId(entity.Id), entity.AirlineId, entity.Name, entity.MilesPerDollar);

    public async Task<LoyaltyProgramAggregate?> GetByIdAsync(
        LoyaltyProgramId  id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.LoyaltyPrograms
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IEnumerable<LoyaltyProgramAggregate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.LoyaltyPrograms
            .AsNoTracking()
            .OrderBy(e => e.Name)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task<LoyaltyProgramAggregate?> GetByAirlineAsync(
        int               airlineId,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.LoyaltyPrograms
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.AirlineId == airlineId, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task AddAsync(
        LoyaltyProgramAggregate loyaltyProgram,
        CancellationToken       cancellationToken = default)
    {
        var entity = new LoyaltyProgramEntity
        {
            AirlineId      = loyaltyProgram.AirlineId,
            Name           = loyaltyProgram.Name,
            MilesPerDollar = loyaltyProgram.MilesPerDollar
        };
        await _context.LoyaltyPrograms.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(
        LoyaltyProgramAggregate loyaltyProgram,
        CancellationToken       cancellationToken = default)
    {
        var entity = await _context.LoyaltyPrograms
            .FirstOrDefaultAsync(e => e.Id == loyaltyProgram.Id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"LoyaltyProgramEntity with id {loyaltyProgram.Id.Value} not found.");

        
        entity.Name           = loyaltyProgram.Name;
        entity.MilesPerDollar = loyaltyProgram.MilesPerDollar;

        _context.LoyaltyPrograms.Update(entity);
    }

    public async Task DeleteAsync(
        LoyaltyProgramId  id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.LoyaltyPrograms
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"LoyaltyProgramEntity with id {id.Value} not found.");

        _context.LoyaltyPrograms.Remove(entity);
    }
}
