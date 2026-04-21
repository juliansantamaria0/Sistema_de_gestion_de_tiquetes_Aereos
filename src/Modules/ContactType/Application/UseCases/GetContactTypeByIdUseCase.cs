namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ContactType.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ContactType.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ContactType.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ContactType.Domain.ValueObject;

public sealed class GetContactTypeByIdUseCase
{
    private readonly IContactTypeRepository _repository;

    public GetContactTypeByIdUseCase(IContactTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<ContactTypeAggregate?> ExecuteAsync(
        int               id,
        CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(new ContactTypeId(id), cancellationToken);
}
