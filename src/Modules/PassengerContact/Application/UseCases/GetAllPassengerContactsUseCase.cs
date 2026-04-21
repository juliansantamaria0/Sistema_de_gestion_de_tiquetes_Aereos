namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerContact.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerContact.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerContact.Domain.Repositories;

public sealed class GetAllPassengerContactsUseCase
{
    private readonly IPassengerContactRepository _repository;

    public GetAllPassengerContactsUseCase(IPassengerContactRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<PassengerContactAggregate>> ExecuteAsync(
        CancellationToken cancellationToken = default)
        => await _repository.GetAllAsync(cancellationToken);
}
