namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Application.UseCases;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreateFlightSeatUseCase
{
    private readonly IFlightSeatRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly AppDbContext _context;

    public CreateFlightSeatUseCase(IFlightSeatRepository repository, IUnitOfWork unitOfWork, AppDbContext context)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public async Task<FlightSeatAggregate> ExecuteAsync(
        int scheduledFlightId,
        int seatMapId,
        int seatStatusId,
        CancellationToken cancellationToken = default)
    {
        var scheduledFlight = await _context.ScheduledFlights
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == scheduledFlightId, cancellationToken)
            ?? throw new InvalidOperationException($"No existe el vuelo programado con id {scheduledFlightId}.");

        var seatMap = await _context.SeatMaps
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == seatMapId, cancellationToken)
            ?? throw new InvalidOperationException($"No existe el mapa de asiento con id {seatMapId}.");

        if (!await _context.SeatStatuses.AsNoTracking().AnyAsync(x => x.Id == seatStatusId, cancellationToken))
            throw new InvalidOperationException($"No existe el estado de asiento con id {seatStatusId}.");

        var aircraftType = await _context.Aircrafts
            .AsNoTracking()
            .Where(x => x.AircraftId == scheduledFlight.AircraftId)
            .Join(
                _context.AircraftTypes.AsNoTracking(),
                aircraft => aircraft.AircraftTypeId,
                type => type.AircraftTypeId,
                (_, type) => new { type.AircraftTypeId, type.TotalSeats })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new InvalidOperationException("No fue posible determinar el tipo y capacidad de la aeronave del vuelo programado.");

        if (seatMap.AircraftTypeId != aircraftType.AircraftTypeId)
            throw new InvalidOperationException("El asiento seleccionado no pertenece al tipo de aeronave asignado al vuelo programado.");

        var assignedSeats = await _context.FlightSeats
            .AsNoTracking()
            .CountAsync(x => x.ScheduledFlightId == scheduledFlightId, cancellationToken);

        if (assignedSeats >= aircraftType.TotalSeats)
            throw new InvalidOperationException("No se pueden registrar más asientos que la capacidad total del avión.");

        var duplicateSeat = await _context.FlightSeats
            .AsNoTracking()
            .AnyAsync(x => x.ScheduledFlightId == scheduledFlightId && x.SeatMapId == seatMapId, cancellationToken);

        if (duplicateSeat)
            throw new InvalidOperationException("Ese asiento ya fue registrado para el vuelo programado.");

        var flightSeat = new FlightSeatAggregate(
            new FlightSeatId(1),
            scheduledFlightId,
            seatMapId,
            seatStatusId,
            DateTime.UtcNow);

        await _repository.AddAsync(flightSeat, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return flightSeat;
    }
}
