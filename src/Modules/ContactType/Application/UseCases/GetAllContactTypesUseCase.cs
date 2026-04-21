namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ContactType.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ContactType.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ContactType.Domain.Repositories;

public sealed class GetAllContactTypesUseCase
{
    private readonly IContactTypeRepository _repository;

    public GetAllContactTypesUseCase(IContactTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ContactTypeAggregate>> ExecuteAsync(
        CancellationToken cancellationToken = default)
        => await _repository.GetAllAsync(cancellationToken);
}
