namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreateFlightCancellationUseCase
{
    private readonly IFlightCancellationRepository _repository;
    private readonly IUnitOfWork                   _unitOfWork;

    public CreateFlightCancellationUseCase(IFlightCancellationRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<FlightCancellationAggregate> ExecuteAsync(
        int               scheduledFlightId,
        int               cancellationReasonId,
        string?           notes,
        CancellationToken cancellationToken = default)
    {
        
        
        
        var flightCancellation = new FlightCancellationAggregate(
            new FlightCancellationId(0),
            scheduledFlightId,
            cancellationReasonId,
            DateTime.UtcNow,
            notes);

        await _repository.AddAsync(flightCancellation, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return flightCancellation;
    }
}
