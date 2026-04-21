namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.CabinClass.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CabinClass.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CabinClass.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CabinClass.Domain.ValueObject;

public sealed class GetCabinClassByIdUseCase
{
    private readonly ICabinClassRepository _repository;

    public GetCabinClassByIdUseCase(ICabinClassRepository repository)
    {
        _repository = repository;
    }

    public async Task<CabinClassAggregate?> ExecuteAsync(
        int id, CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(new CabinClassId(id), cancellationToken);
}
