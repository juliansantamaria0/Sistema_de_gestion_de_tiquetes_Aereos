namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Domain.ValueObject;

public sealed class GetSeatMapByIdUseCase
{
    private readonly ISeatMapRepository _repository;

    public GetSeatMapByIdUseCase(ISeatMapRepository repository)
    {
        _repository = repository;
    }

    public async Task<SeatMapAggregate?> ExecuteAsync(
        int               id,
        CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(new SeatMapId(id), cancellationToken);
}
