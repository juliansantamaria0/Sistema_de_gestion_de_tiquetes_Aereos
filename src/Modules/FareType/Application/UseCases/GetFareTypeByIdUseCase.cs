namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.Domain.ValueObject;

public sealed class GetFareTypeByIdUseCase
{
    private readonly IFareTypeRepository _repository;

    public GetFareTypeByIdUseCase(IFareTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<FareTypeAggregate?> ExecuteAsync(
        int id, CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(new FareTypeId(id), cancellationToken);
}
