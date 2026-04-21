namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatus.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatus.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatus.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class UpdateFlightStatusUseCase
{
    private readonly IFlightStatusRepository _repository;
    private readonly IUnitOfWork             _unitOfWork;

    public UpdateFlightStatusUseCase(IFlightStatusRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int               id,
        string            newName,
        CancellationToken cancellationToken = default)
    {
        var flightStatus = await _repository.GetByIdAsync(new FlightStatusId(id), cancellationToken)
            ?? throw new KeyNotFoundException($"FlightStatus with id {id} was not found.");

        flightStatus.UpdateName(newName);
        await _repository.UpdateAsync(flightStatus, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
