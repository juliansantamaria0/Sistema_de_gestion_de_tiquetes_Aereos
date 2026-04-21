namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class FlightSeatRepository : IFlightSeatRepository
{
    private readonly AppDbContext _context;

    // seat_status_id correspondiente a AVAILABLE.
    // Se filtra por nombre en GetAvailableByFlightAsync usando una subconsulta
    // para no acoplar el repositorio a un valor numérico hardcodeado.
    private const string AvailableStatusName = "AVAILABLE";

    public FlightSeatRepository(AppDbContext context)
    {
        _context = context;
    }

    // ── Mapeos privados ───────────────────────────────────────────────────────

    private static FlightSeatAggregate ToDomain(FlightSeatEntity entity)
        => new(
            new FlightSeatId(entity.Id),
            entity.ScheduledFlightId,
            entity.SeatMapId,
            entity.SeatStatusId,
            entity.CreatedAt,
            entity.UpdatedAt);

    // ── Operaciones ───────────────────────────────────────────────────────────

    public async Task<FlightSeatAggregate?> GetByIdAsync(
        FlightSeatId      id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.FlightSeats
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IEnumerable<FlightSeatAggregate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.FlightSeats
            .AsNoTracking()
            .OrderBy(e => e.ScheduledFlightId)
            .ThenBy(e => e.SeatMapId)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task<IEnumerable<FlightSeatAggregate>> GetByFlightAsync(
        int               scheduledFlightId,
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.FlightSeats
            .AsNoTracking()
            .Where(e => e.ScheduledFlightId == scheduledFlightId)
            .OrderBy(e => e.SeatMapId)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task<IEnumerable<FlightSeatAggregate>> GetAvailableByFlightAsync(
        int               scheduledFlightId,
        CancellationToken cancellationToken = default)
    {
        // Filtra por el nombre del estado para evitar hardcodear un ID numérico.
        var availableStatusId = await _context.SeatStatuses
            .AsNoTracking()
            .Where(ss => ss.Name == AvailableStatusName)
            .Select(ss => ss.Id)
            .FirstOrDefaultAsync(cancellationToken);

        var entities = await _context.FlightSeats
            .AsNoTracking()
            .Where(e => e.ScheduledFlightId == scheduledFlightId
                     && e.SeatStatusId       == availableStatusId)
            .OrderBy(e => e.SeatMapId)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task AddAsync(
        FlightSeatAggregate flightSeat,
        CancellationToken   cancellationToken = default)
    {
        var entity = new FlightSeatEntity
        {
            ScheduledFlightId = flightSeat.ScheduledFlightId,
            SeatMapId         = flightSeat.SeatMapId,
            SeatStatusId      = flightSeat.SeatStatusId,
            CreatedAt         = flightSeat.CreatedAt,
            UpdatedAt         = flightSeat.UpdatedAt
        };
        await _context.FlightSeats.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(
        FlightSeatAggregate flightSeat,
        CancellationToken   cancellationToken = default)
    {
        var entity = await _context.FlightSeats
            .FirstOrDefaultAsync(e => e.Id == flightSeat.Id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"FlightSeatEntity with id {flightSeat.Id.Value} not found.");

        // Solo seat_status_id y updated_at son mutables.
        // scheduled_flight_id y seat_map_id forman la clave de negocio.
        entity.SeatStatusId = flightSeat.SeatStatusId;
        entity.UpdatedAt    = flightSeat.UpdatedAt;

        _context.FlightSeats.Update(entity);
    }

    public async Task DeleteAsync(
        FlightSeatId      id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.FlightSeats
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"FlightSeatEntity with id {id.Value} not found.");

        _context.FlightSeats.Remove(entity);
    }
}
