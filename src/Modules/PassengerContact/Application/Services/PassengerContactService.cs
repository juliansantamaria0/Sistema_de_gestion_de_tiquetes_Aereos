namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerContact.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerContact.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerContact.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerContact.Domain.Aggregate;

public sealed class PassengerContactService : IPassengerContactService
{
    private readonly CreatePassengerContactUseCase           _create;
    private readonly DeletePassengerContactUseCase           _delete;
    private readonly GetAllPassengerContactsUseCase          _getAll;
    private readonly GetPassengerContactByIdUseCase          _getById;
    private readonly UpdatePassengerContactUseCase           _update;
    private readonly GetPassengerContactsByPassengerUseCase  _getByPassenger;

    public PassengerContactService(
        CreatePassengerContactUseCase          create,
        DeletePassengerContactUseCase          delete,
        GetAllPassengerContactsUseCase         getAll,
        GetPassengerContactByIdUseCase         getById,
        UpdatePassengerContactUseCase          update,
        GetPassengerContactsByPassengerUseCase getByPassenger)
    {
        _create         = create;
        _delete         = delete;
        _getAll         = getAll;
        _getById        = getById;
        _update         = update;
        _getByPassenger = getByPassenger;
    }

    public async Task<PassengerContactDto> CreateAsync(
        int passengerId, int contactTypeId, string fullName,
        string phone, string? relationship,
        CancellationToken cancellationToken = default)
    {
        var agg = await _create.ExecuteAsync(
            passengerId, contactTypeId, fullName, phone, relationship, cancellationToken);
        return ToDto(agg);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await _delete.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<PassengerContactDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var list = await _getAll.ExecuteAsync(cancellationToken);
        return list.Select(ToDto);
    }

    public async Task<PassengerContactDto?> GetByIdAsync(
        int id, CancellationToken cancellationToken = default)
    {
        var agg = await _getById.ExecuteAsync(id, cancellationToken);
        return agg is null ? null : ToDto(agg);
    }

    public async Task UpdateAsync(
        int id, string fullName, string phone, string? relationship,
        CancellationToken cancellationToken = default)
        => await _update.ExecuteAsync(id, fullName, phone, relationship, cancellationToken);

    public async Task<IEnumerable<PassengerContactDto>> GetByPassengerAsync(
        int passengerId, CancellationToken cancellationToken = default)
    {
        var list = await _getByPassenger.ExecuteAsync(passengerId, cancellationToken);
        return list.Select(ToDto);
    }

    private static PassengerContactDto ToDto(PassengerContactAggregate agg)
        => new(agg.Id.Value, agg.PassengerId, agg.ContactTypeId,
               agg.FullName, agg.Phone, agg.Relationship);
}
