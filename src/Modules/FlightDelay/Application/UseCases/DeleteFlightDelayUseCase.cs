namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class DeleteFlightDelayUseCase
{
    private readonly IFlightDelayRepository _repository;
    private readonly IUnitOfWork            _unitOfWork;

    public DeleteFlightDelayUseCase(IFlightDelayRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(new FlightDelayId(id), cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
