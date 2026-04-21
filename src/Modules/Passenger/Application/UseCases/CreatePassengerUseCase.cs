namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreatePassengerUseCase
{
    private readonly IPassengerRepository _repository;
    private readonly IUnitOfWork          _unitOfWork;

    public CreatePassengerUseCase(IPassengerRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PassengerAggregate> ExecuteAsync(
        int               personId,
        string?           frequentFlyerNumber,
        int?              nationalityId,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var passenger = new PassengerAggregate(
            new PassengerId(1), personId, frequentFlyerNumber, nationalityId, now);

        await _repository.AddAsync(passenger, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return passenger;
    }
}
