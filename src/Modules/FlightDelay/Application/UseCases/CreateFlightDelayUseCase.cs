namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreateFlightDelayUseCase
{
    private readonly IFlightDelayRepository _repository;
    private readonly IUnitOfWork            _unitOfWork;

    public CreateFlightDelayUseCase(IFlightDelayRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<FlightDelayAggregate> ExecuteAsync(
        int               scheduledFlightId,
        int               delayReasonId,
        int               delayMinutes,
        CancellationToken cancellationToken = default)
    {
        
        var flightDelay = new FlightDelayAggregate(
            new FlightDelayId(0),
            scheduledFlightId,
            delayReasonId,
            delayMinutes,
            DateTime.UtcNow);

        await _repository.AddAsync(flightDelay, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return flightDelay;
    }
}
