namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.BoardingPass.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.BoardingPass.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.BoardingPass.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.BoardingPass.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.BoardingPass.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class BoardingPassRepository : IBoardingPassRepository
{
    private readonly AppDbContext _context;

    public BoardingPassRepository(AppDbContext context)
    {
        _context = context;
    }

    private static BoardingPassAggregate ToDomain(BoardingPassEntity entity)
        => new(
            new BoardingPassId(entity.Id),
            entity.BoardingPassCode,
            entity.CheckInId,
            entity.GateId,
            entity.BoardingGroup,
            entity.FlightSeatId,
            entity.IssuedAt);

    public async Task<BoardingPassAggregate?> GetByIdAsync(
        BoardingPassId    id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.BoardingPasses
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IEnumerable<BoardingPassAggregate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.BoardingPasses
            .AsNoTracking()
            .OrderByDescending(e => e.IssuedAt)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task<BoardingPassAggregate?> GetByCheckInAsync(
        int               checkInId,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.BoardingPasses
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.CheckInId == checkInId, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<BoardingPassAggregate?> GetByCodeAsync(
        string            normalizedCode,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.BoardingPasses
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.BoardingPassCode == normalizedCode, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<bool> BoardingPassCodeExistsAsync(
        string            normalizedCode,
        CancellationToken cancellationToken = default) =>
        await _context.BoardingPasses
            .AsNoTracking()
            .AnyAsync(e => e.BoardingPassCode == normalizedCode, cancellationToken);

    public async Task AddAsync(
        BoardingPassAggregate boardingPass,
        CancellationToken     cancellationToken = default)
    {
        var entity = new BoardingPassEntity
        {
            BoardingPassCode = boardingPass.BoardingPassCode,
            CheckInId        = boardingPass.CheckInId,
            GateId           = boardingPass.GateId,
            BoardingGroup    = boardingPass.BoardingGroup,
            FlightSeatId     = boardingPass.FlightSeatId,
            IssuedAt         = boardingPass.IssuedAt
        };
        await _context.BoardingPasses.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(
        BoardingPassAggregate boardingPass,
        CancellationToken     cancellationToken = default)
    {
        var entity = await _context.BoardingPasses
            .FirstOrDefaultAsync(e => e.Id == boardingPass.Id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"No se encontró el pase de abordar con id {boardingPass.Id.Value}.");

        entity.GateId        = boardingPass.GateId;
        entity.BoardingGroup = boardingPass.BoardingGroup;

        _context.BoardingPasses.Update(entity);
    }

    public async Task DeleteAsync(
        BoardingPassId    id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.BoardingPasses
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"No se encontró el pase de abordar con id {id.Value}.");

        _context.BoardingPasses.Remove(entity);
    }
}
