namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatus.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatus.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatus.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class DeleteFlightStatusUseCase
{
    private readonly IFlightStatusRepository _repository;
    private readonly IUnitOfWork             _unitOfWork;

    public DeleteFlightStatusUseCase(IFlightStatusRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(new FlightStatusId(id), cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
