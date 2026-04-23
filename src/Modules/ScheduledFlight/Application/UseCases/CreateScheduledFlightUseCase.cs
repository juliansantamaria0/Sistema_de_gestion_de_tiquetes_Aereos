namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreateScheduledFlightUseCase
{
    private readonly IScheduledFlightRepository _repository;
    private readonly IUnitOfWork                _unitOfWork;

    public CreateScheduledFlightUseCase(IScheduledFlightRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ScheduledFlightAggregate> ExecuteAsync(
        CreateScheduledFlightRequest request,
        CancellationToken            cancellationToken = default)
    {
        // ScheduledFlightId(1) es placeholder; EF Core asigna el Id real al insertar.
        var scheduledFlight = new ScheduledFlightAggregate(
            new ScheduledFlightId(await GetNextIdAsync(cancellationToken)),
            request.BaseFlightId,
            request.AircraftId,
            request.GateId,
            request.DepartureDate,
            request.DepartureTime,
            request.EstimatedArrivalDatetime,
            request.FlightStatusId,
            DateTime.UtcNow);

        await _repository.AddAsync(scheduledFlight, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return scheduledFlight;
    }

    private async Task<int> GetNextIdAsync(CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return items.Select(x => x.Id.Value).DefaultIfEmpty(0).Max() + 1;
    }
}
