namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreateFlightCrewUseCase
{
    private readonly IFlightCrewRepository _repository;
    private readonly IUnitOfWork           _unitOfWork;

    public CreateFlightCrewUseCase(IFlightCrewRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<FlightCrewAggregate> ExecuteAsync(
        int               scheduledFlightId,
        int               employeeId,
        int               crewRoleId,
        CancellationToken cancellationToken = default)
    {
        // FlightCrewId(1) es placeholder; EF Core asigna el Id real al insertar.
        var flightCrew = new FlightCrewAggregate(
            new FlightCrewId(1),
            scheduledFlightId,
            employeeId,
            crewRoleId,
            DateTime.UtcNow);

        await _repository.AddAsync(flightCrew, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return flightCrew;
    }
}
