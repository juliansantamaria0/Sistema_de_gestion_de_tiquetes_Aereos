namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Application.UseCases;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class SearchFlightsForBookingUseCase
{
    private readonly AppDbContext _context;

    public SearchFlightsForBookingUseCase(AppDbContext context)
    {
        _context = context;
    }

    public record FlightInfoDto(
        int       ScheduledFlightId,
        int       BaseFlightId,
        string    FlightCode,
        DateOnly  DepartureDate,
        TimeOnly  DepartureTime,
        DateTime  EstimatedArrivalDatetime,
        int       AvailableSeats);

    public async Task<IEnumerable<FlightInfoDto>> ExecuteAsync(
        int               originAirportId,
        int               destinationAirportId,
        CancellationToken cancellationToken = default)
    {
        var result = await _context.ScheduledFlights
            .AsNoTracking()
            .Join(_context.BaseFlights.AsNoTracking(),
                sf => sf.BaseFlightId,
                bf => bf.Id,
                (sf, bf) => new { sf, bf })
            .Join(_context.Routes.AsNoTracking(),
                x => x.bf.RouteId,
                route => route.Id,
                (x, route) => new { x.sf, x.bf, route })
            .Where(x => x.route.OriginAirportId == originAirportId &&
                        x.route.DestinationAirportId == destinationAirportId)
            .GroupJoin(_context.FlightSeats.AsNoTracking()
                    .Where(fs => fs.SeatStatusId == (
                        _context.SeatStatuses.AsNoTracking()
                            .Where(ss => ss.Name == "AVAILABLE")
                            .Select(ss => ss.Id)
                            .FirstOrDefault())),
                x => x.sf.Id,
                fs => fs.ScheduledFlightId,
                (x, seats) => new FlightInfoDto(
                    x.sf.Id,
                    x.bf.Id,
                    x.bf.FlightCode,
                    x.sf.DepartureDate,
                    x.sf.DepartureTime,
                    x.sf.EstimatedArrivalDatetime,
                    seats.Count()))
            .OrderByDescending(f => f.DepartureDate)
            .ThenBy(f => f.DepartureTime)
            .ToListAsync(cancellationToken);

        return result;
    }
}
