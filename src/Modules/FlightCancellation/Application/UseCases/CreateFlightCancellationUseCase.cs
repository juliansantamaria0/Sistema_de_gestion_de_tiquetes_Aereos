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
        // FlightCancellationId(1) es placeholder; EF Core asigna el Id real al insertar.
        // La UNIQUE constraint sobre scheduled_flight_id garantiza que no se
        // pueda cancelar el mismo vuelo dos veces a nivel de BD.
        var flightCancellation = new FlightCancellationAggregate(
            new FlightCancellationId(await GetNextIdAsync(cancellationToken)),
            scheduledFlightId,
            cancellationReasonId,
            DateTime.UtcNow,
            notes);

        await _repository.AddAsync(flightCancellation, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return flightCancellation;
    }

    private async Task<int> GetNextIdAsync(CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return items.Select(x => x.Id.Value).DefaultIfEmpty(0).Max() + 1;
    }
}
