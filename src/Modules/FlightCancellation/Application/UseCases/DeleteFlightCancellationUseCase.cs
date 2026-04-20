namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class DeleteFlightCancellationUseCase
{
    private readonly IFlightCancellationRepository _repository;
    private readonly IUnitOfWork                   _unitOfWork;

    public DeleteFlightCancellationUseCase(IFlightCancellationRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(new FlightCancellationId(id), cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
