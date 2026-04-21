namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Domain.ValueObject;

public sealed class GetPassengerByIdUseCase
{
    private readonly IPassengerRepository _repository;

    public GetPassengerByIdUseCase(IPassengerRepository repository)
    {
        _repository = repository;
    }

    public async Task<PassengerAggregate?> ExecuteAsync(
        int               id,
        CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(new PassengerId(id), cancellationToken);
}
