namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatStatus.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatStatus.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatStatus.Domain.Repositories;

public sealed class GetAllSeatStatusesUseCase
{
    private readonly ISeatStatusRepository _repository;

    public GetAllSeatStatusesUseCase(ISeatStatusRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<SeatStatusAggregate>> ExecuteAsync(
        CancellationToken cancellationToken = default)
        => await _repository.GetAllAsync(cancellationToken);
}
