namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.Domain.Repositories;

public sealed class GetAllFareTypesUseCase
{
    private readonly IFareTypeRepository _repository;

    public GetAllFareTypesUseCase(IFareTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<FareTypeAggregate>> ExecuteAsync(
        CancellationToken cancellationToken = default)
        => await _repository.GetAllAsync(cancellationToken);
}
