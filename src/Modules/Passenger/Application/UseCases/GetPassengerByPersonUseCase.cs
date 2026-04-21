namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Domain.Repositories;

/// <summary>
/// Obtiene el perfil de pasajero asociado a una persona.
/// La UNIQUE sobre person_id garantiza como máximo un resultado.
/// </summary>
public sealed class GetPassengerByPersonUseCase
{
    private readonly IPassengerRepository _repository;

    public GetPassengerByPersonUseCase(IPassengerRepository repository)
    {
        _repository = repository;
    }

    public async Task<PassengerAggregate?> ExecuteAsync(
        int               personId,
        CancellationToken cancellationToken = default)
        => await _repository.GetByPersonAsync(personId, cancellationToken);
}
