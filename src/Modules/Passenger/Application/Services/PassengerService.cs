namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Infrastructure;

public sealed class PassengerService : IPassengerService
{
    private readonly CreatePassengerUseCase      _create;
    private readonly DeletePassengerUseCase      _delete;
    private readonly GetAllPassengersUseCase     _getAll;
    private readonly GetPassengerByIdUseCase     _getById;
    private readonly UpdatePassengerUseCase      _update;
    private readonly GetPassengerByPersonUseCase _getByPerson;

    public PassengerService(
        CreatePassengerUseCase     create,
        DeletePassengerUseCase     delete,
        GetAllPassengersUseCase    getAll,
        GetPassengerByIdUseCase    getById,
        UpdatePassengerUseCase     update,
        GetPassengerByPersonUseCase getByPerson)
    {
        _create      = create;
        _delete      = delete;
        _getAll      = getAll;
        _getById     = getById;
        _update      = update;
        _getByPerson = getByPerson;
    }

    public async Task<PassengerDto> CreateAsync(
        int               personId,
        string?           frequentFlyerNumber,
        int?              nationalityId,
        CancellationToken cancellationToken = default)
    {
        var agg = await _create.ExecuteAsync(
            personId, frequentFlyerNumber, nationalityId, cancellationToken);
        return ToDto(agg);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await _delete.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<PassengerDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        if (CurrentUser.IsAuthenticated && CurrentUser.PersonId.HasValue)
        {
            var personId = CurrentUser.PersonId.Value;
            var single   = await _getByPerson.ExecuteAsync(personId, cancellationToken);
            if (single is not null)
                return [ToDto(single)];

            // Cuentas antiguas o datos incompletos: un cliente debe tener ficha de pasajero vinculada a su persona.
            try
            {
                var created = await _create.ExecuteAsync(personId, null, null, cancellationToken);
                return [ToDto(created)];
            }
            catch (InvalidOperationException)
            {
                // Carrera: otro hilo pudo crear el pasajero.
                var retry = await _getByPerson.ExecuteAsync(personId, cancellationToken);
                return retry is null ? [] : [ToDto(retry)];
            }
        }
        var list = await _getAll.ExecuteAsync(cancellationToken);
        return list.Select(ToDto);
    }

    public async Task<PassengerDto?> GetByIdAsync(
        int               id,
        CancellationToken cancellationToken = default)
    {
        var agg = await _getById.ExecuteAsync(id, cancellationToken);
        return agg is null ? null : ToDto(agg);
    }

    public async Task UpdateAsync(
        int               id,
        string?           frequentFlyerNumber,
        int?              nationalityId,
        CancellationToken cancellationToken = default)
        => await _update.ExecuteAsync(id, frequentFlyerNumber, nationalityId, cancellationToken);

    public async Task<PassengerDto?> GetByPersonAsync(
        int               personId,
        CancellationToken cancellationToken = default)
    {
        var agg = await _getByPerson.ExecuteAsync(personId, cancellationToken);
        return agg is null ? null : ToDto(agg);
    }

    private static PassengerDto ToDto(PassengerAggregate agg)
        => new(
            agg.Id.Value,
            agg.PersonId,
            agg.FrequentFlyerNumber,
            agg.NationalityId,
            agg.CreatedAt,
            agg.UpdatedAt);
}
