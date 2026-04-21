namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class UpdateScheduledFlightUseCase
{
    private readonly IScheduledFlightRepository _repository;
    private readonly IUnitOfWork                _unitOfWork;

    public UpdateScheduledFlightUseCase(IScheduledFlightRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int                          id,
        UpdateScheduledFlightRequest request,
        CancellationToken            cancellationToken = default)
    {
        var scheduledFlight = await _repository.GetByIdAsync(new ScheduledFlightId(id), cancellationToken)
            ?? throw new KeyNotFoundException($"ScheduledFlight with id {id} was not found.");

        scheduledFlight.Update(
            request.AircraftId,
            request.GateId,
            request.DepartureDate,
            request.DepartureTime,
            request.EstimatedArrivalDatetime,
            request.FlightStatusId);

        await _repository.UpdateAsync(scheduledFlight, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
