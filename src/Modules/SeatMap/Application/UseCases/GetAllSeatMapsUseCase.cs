namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Domain.Repositories;

public sealed class GetAllSeatMapsUseCase
{
    private readonly ISeatMapRepository _repository;

    public GetAllSeatMapsUseCase(ISeatMapRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<SeatMapAggregate>> ExecuteAsync(
        CancellationToken cancellationToken = default)
        => await _repository.GetAllAsync(cancellationToken);
}
