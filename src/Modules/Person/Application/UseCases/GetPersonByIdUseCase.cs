namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Domain.ValueObject;

public sealed class GetPersonByIdUseCase
{
    private readonly IPersonRepository _repository;

    public GetPersonByIdUseCase(IPersonRepository repository)
    {
        _repository = repository;
    }

    public async Task<PersonAggregate?> ExecuteAsync(
        int               id,
        CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(new PersonId(id), cancellationToken);
}
