namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ContactType.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ContactType.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ContactType.Application.UseCases;

public sealed class ContactTypeService : IContactTypeService
{
    private readonly CreateContactTypeUseCase   _create;
    private readonly DeleteContactTypeUseCase   _delete;
    private readonly GetAllContactTypesUseCase  _getAll;
    private readonly GetContactTypeByIdUseCase  _getById;
    private readonly UpdateContactTypeUseCase   _update;

    public ContactTypeService(
        CreateContactTypeUseCase  create,
        DeleteContactTypeUseCase  delete,
        GetAllContactTypesUseCase getAll,
        GetContactTypeByIdUseCase getById,
        UpdateContactTypeUseCase  update)
    {
        _create  = create;
        _delete  = delete;
        _getAll  = getAll;
        _getById = getById;
        _update  = update;
    }

    public async Task<ContactTypeDto> CreateAsync(
        string name, CancellationToken cancellationToken = default)
    {
        var agg = await _create.ExecuteAsync(name, cancellationToken);
        return new ContactTypeDto(agg.Id.Value, agg.Name);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await _delete.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<ContactTypeDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var list = await _getAll.ExecuteAsync(cancellationToken);
        return list.Select(a => new ContactTypeDto(a.Id.Value, a.Name));
    }

    public async Task<ContactTypeDto?> GetByIdAsync(
        int id, CancellationToken cancellationToken = default)
    {
        var agg = await _getById.ExecuteAsync(id, cancellationToken);
        return agg is null ? null : new ContactTypeDto(agg.Id.Value, agg.Name);
    }

    public async Task UpdateAsync(
        int id, string name, CancellationToken cancellationToken = default)
        => await _update.ExecuteAsync(id, name, cancellationToken);
}
