namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatusHistory.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatusHistory.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatusHistory.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class DeleteFlightStatusHistoryUseCase
{
    private readonly IFlightStatusHistoryRepository _repository;
    private readonly IUnitOfWork                    _unitOfWork;

    public DeleteFlightStatusHistoryUseCase(
        IFlightStatusHistoryRepository repository,
        IUnitOfWork                    unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(new FlightStatusHistoryId(id), cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
