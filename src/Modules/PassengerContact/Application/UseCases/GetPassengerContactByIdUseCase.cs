namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerContact.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerContact.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerContact.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerContact.Domain.ValueObject;

public sealed class GetPassengerContactByIdUseCase
{
    private readonly IPassengerContactRepository _repository;

    public GetPassengerContactByIdUseCase(IPassengerContactRepository repository)
    {
        _repository = repository;
    }

    public async Task<PassengerContactAggregate?> ExecuteAsync(
        int id, CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(new PassengerContactId(id), cancellationToken);
}
