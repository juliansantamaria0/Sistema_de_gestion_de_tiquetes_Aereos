namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class UpdatePassengerUseCase
{
    private readonly IPassengerRepository _repository;
    private readonly IUnitOfWork          _unitOfWork;

    public UpdatePassengerUseCase(IPassengerRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int               id,
        string?           frequentFlyerNumber,
        int?              nationalityId,
        CancellationToken cancellationToken = default)
    {
        var passenger = await _repository.GetByIdAsync(new PassengerId(id), cancellationToken)
            ?? throw new KeyNotFoundException($"Passenger with id {id} was not found.");

        passenger.Update(frequentFlyerNumber, nationalityId);
        await _repository.UpdateAsync(passenger, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
