namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Domain.Aggregate;

public sealed class PersonService : IPersonService
{
    private readonly CreatePersonUseCase       _create;
    private readonly DeletePersonUseCase       _delete;
    private readonly GetAllPersonsUseCase      _getAll;
    private readonly GetPersonByIdUseCase      _getById;
    private readonly UpdatePersonUseCase       _update;
    private readonly GetPersonByDocumentUseCase _getByDocument;

    public PersonService(
        CreatePersonUseCase        create,
        DeletePersonUseCase        delete,
        GetAllPersonsUseCase       getAll,
        GetPersonByIdUseCase       getById,
        UpdatePersonUseCase        update,
        GetPersonByDocumentUseCase getByDocument)
    {
        _create        = create;
        _delete        = delete;
        _getAll        = getAll;
        _getById       = getById;
        _update        = update;
        _getByDocument = getByDocument;
    }

    public async Task<PersonDto> CreateAsync(
        int               documentTypeId,
        string            documentNumber,
        string            firstName,
        string            lastName,
        DateOnly?         birthDate,
        int?              genderId,
        CancellationToken cancellationToken = default)
    {
        var agg = await _create.ExecuteAsync(
            documentTypeId, documentNumber, firstName, lastName, birthDate, genderId,
            cancellationToken);
        return ToDto(agg);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await _delete.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<PersonDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var list = await _getAll.ExecuteAsync(cancellationToken);
        return list.Select(ToDto);
    }

    public async Task<PersonDto?> GetByIdAsync(
        int               id,
        CancellationToken cancellationToken = default)
    {
        var agg = await _getById.ExecuteAsync(id, cancellationToken);
        return agg is null ? null : ToDto(agg);
    }

    public async Task UpdateAsync(
        int               id,
        string            firstName,
        string            lastName,
        DateOnly?         birthDate,
        int?              genderId,
        CancellationToken cancellationToken = default)
        => await _update.ExecuteAsync(id, firstName, lastName, birthDate, genderId, cancellationToken);

    public async Task<PersonDto?> GetByDocumentAsync(
        int               documentTypeId,
        string            documentNumber,
        CancellationToken cancellationToken = default)
    {
        var agg = await _getByDocument.ExecuteAsync(documentTypeId, documentNumber, cancellationToken);
        return agg is null ? null : ToDto(agg);
    }

    private static PersonDto ToDto(PersonAggregate agg)
        => new(
            agg.Id.Value,
            agg.DocumentTypeId,
            agg.DocumentNumber,
            agg.FirstName,
            agg.LastName,
            agg.BirthDate,
            agg.GenderId,
            agg.CreatedAt,
            agg.UpdatedAt);
}
