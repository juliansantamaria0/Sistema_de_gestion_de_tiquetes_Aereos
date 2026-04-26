namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Application.UseCases;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Constants;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreateScheduledFlightUseCase
{
    private readonly IScheduledFlightRepository _repository;
    private readonly IUnitOfWork                _unitOfWork;
    private readonly AppDbContext               _context;

    public CreateScheduledFlightUseCase(
        IScheduledFlightRepository repository,
        IUnitOfWork                unitOfWork,
        AppDbContext               context)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _context    = context;
    }

    public async Task<ScheduledFlightAggregate> ExecuteAsync(
        CreateScheduledFlightRequest request,
        CancellationToken            cancellationToken = default)
    {
        var scheduledFlight = ScheduledFlightAggregate.Create(
            request.BaseFlightId,
            request.AircraftId,
            request.GateId,
            request.DepartureDate,
            request.DepartureTime,
            request.EstimatedArrivalDatetime,
            request.FlightStatusId);

        await _repository.AddAsync(scheduledFlight, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        // Recuperar el ID real asignado por la base de datos
        var savedId = await _context.ScheduledFlights.AsNoTracking()
            .Where(sf => sf.AircraftId     == request.AircraftId
                      && sf.BaseFlightId   == request.BaseFlightId
                      && sf.DepartureDate  == request.DepartureDate
                      && sf.DepartureTime  == request.DepartureTime)
            .Select(sf => sf.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (savedId > 0)
            await CreateFlightSeatsAsync(savedId, request.AircraftId, cancellationToken);

        return scheduledFlight;
    }

    private async Task CreateFlightSeatsAsync(
        int               scheduledFlightId,
        int               aircraftId,
        CancellationToken ct)
    {
        var aircraftTypeId = await _context.Aircrafts.AsNoTracking()
            .Where(a => a.AircraftId == aircraftId)
            .Select(a => a.AircraftTypeId)
            .FirstOrDefaultAsync(ct);

        if (aircraftTypeId == 0) return;

        var seatMaps = await _context.SeatMaps.AsNoTracking()
            .Where(sm => sm.AircraftTypeId == aircraftTypeId)
            .ToListAsync(ct);

        if (seatMaps.Count == 0) return;

        var availableStatusId = await _context.SeatStatuses.AsNoTracking()
            .Where(s => s.Name == SeatStatusNames.Available)
            .Select(s => s.Id)
            .FirstOrDefaultAsync(ct);

        if (availableStatusId == 0) return;

        var seats = seatMaps.Select(sm => new FlightSeatEntity
        {
            ScheduledFlightId = scheduledFlightId,
            SeatMapId         = sm.Id,
            SeatStatusId      = availableStatusId,
            CreatedAt         = DateTime.UtcNow,
            Version           = []
        }).ToList();

        await _context.FlightSeats.AddRangeAsync(seats, ct);
        await _context.SaveChangesAsync(ct);
    }
}
