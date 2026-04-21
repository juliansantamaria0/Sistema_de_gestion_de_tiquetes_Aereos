namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gender.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gender.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gender.Domain.Repositories;

public sealed class GetAllGendersUseCase
{
    private readonly IGenderRepository _repository;

    public GetAllGendersUseCase(IGenderRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<GenderAggregate>> ExecuteAsync(
        CancellationToken cancellationToken = default)
        => await _repository.GetAllAsync(cancellationToken);
}
