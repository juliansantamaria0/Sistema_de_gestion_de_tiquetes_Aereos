namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class DeleteScheduledFlightUseCase
{
    private readonly IScheduledFlightRepository _repository;
    private readonly IUnitOfWork                _unitOfWork;

    public DeleteScheduledFlightUseCase(IScheduledFlightRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(new ScheduledFlightId(id), cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
