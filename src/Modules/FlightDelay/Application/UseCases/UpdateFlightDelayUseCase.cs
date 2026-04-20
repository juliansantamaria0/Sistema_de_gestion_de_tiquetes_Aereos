namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

/// <summary>
/// Corrige los minutos de retraso de un registro existente.
/// scheduled_flight_id, delay_reason_id y reported_at son inmutables.
/// </summary>
public sealed class UpdateFlightDelayUseCase
{
    private readonly IFlightDelayRepository _repository;
    private readonly IUnitOfWork            _unitOfWork;

    public UpdateFlightDelayUseCase(IFlightDelayRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int               id,
        int               delayMinutes,
        CancellationToken cancellationToken = default)
    {
        var flightDelay = await _repository.GetByIdAsync(new FlightDelayId(id), cancellationToken)
            ?? throw new KeyNotFoundException($"FlightDelay with id {id} was not found.");

        flightDelay.AdjustDelay(delayMinutes);
        await _repository.UpdateAsync(flightDelay, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
