namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Domain.Repositories;

public sealed class GetAllPassengersUseCase
{
    private readonly IPassengerRepository _repository;

    public GetAllPassengersUseCase(IPassengerRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<PassengerAggregate>> ExecuteAsync(
        CancellationToken cancellationToken = default)
        => await _repository.GetAllAsync(cancellationToken);
}
