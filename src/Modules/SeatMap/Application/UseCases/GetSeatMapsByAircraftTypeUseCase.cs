namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Domain.Repositories;






public sealed class GetSeatMapsByAircraftTypeUseCase
{
    private readonly ISeatMapRepository _repository;

    public GetSeatMapsByAircraftTypeUseCase(ISeatMapRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<SeatMapAggregate>> ExecuteAsync(
        int               aircraftTypeId,
        CancellationToken cancellationToken = default)
        => await _repository.GetByAircraftTypeAsync(aircraftTypeId, cancellationToken);
}
