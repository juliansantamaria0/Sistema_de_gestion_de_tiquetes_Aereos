namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.CabinClass.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CabinClass.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CabinClass.Domain.Repositories;

public sealed class GetAllCabinClassesUseCase
{
    private readonly ICabinClassRepository _repository;

    public GetAllCabinClassesUseCase(ICabinClassRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<CabinClassAggregate>> ExecuteAsync(
        CancellationToken cancellationToken = default)
        => await _repository.GetAllAsync(cancellationToken);
}
