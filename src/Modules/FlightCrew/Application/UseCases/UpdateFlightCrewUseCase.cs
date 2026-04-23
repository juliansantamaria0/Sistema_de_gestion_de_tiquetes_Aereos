namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;






public sealed class UpdateFlightCrewUseCase
{
    private readonly IFlightCrewRepository _repository;
    private readonly IUnitOfWork           _unitOfWork;

    public UpdateFlightCrewUseCase(IFlightCrewRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int               id,
        int               crewRoleId,
        CancellationToken cancellationToken = default)
    {
        var flightCrew = await _repository.GetByIdAsync(new FlightCrewId(id), cancellationToken)
            ?? throw new KeyNotFoundException($"FlightCrew with id {id} was not found.");

        flightCrew.ReassignRole(crewRoleId);
        await _repository.UpdateAsync(flightCrew, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
