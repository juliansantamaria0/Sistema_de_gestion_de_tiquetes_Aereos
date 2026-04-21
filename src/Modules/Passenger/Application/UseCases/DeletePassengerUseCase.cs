namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class DeletePassengerUseCase
{
    private readonly IPassengerRepository _repository;
    private readonly IUnitOfWork          _unitOfWork;

    public DeletePassengerUseCase(IPassengerRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(new PassengerId(id), cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
