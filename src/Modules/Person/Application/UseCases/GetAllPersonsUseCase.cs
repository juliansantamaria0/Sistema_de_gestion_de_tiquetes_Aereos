namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Domain.Repositories;

public sealed class GetAllPersonsUseCase
{
    private readonly IPersonRepository _repository;

    public GetAllPersonsUseCase(IPersonRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<PersonAggregate>> ExecuteAsync(
        CancellationToken cancellationToken = default)
        => await _repository.GetAllAsync(cancellationToken);
}
