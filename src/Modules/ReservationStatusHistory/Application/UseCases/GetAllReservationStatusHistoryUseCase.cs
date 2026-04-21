namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.Domain.Repositories;

public sealed class GetAllReservationStatusHistoryUseCase
{
    private readonly IReservationStatusHistoryRepository _repository;

    public GetAllReservationStatusHistoryUseCase(IReservationStatusHistoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ReservationStatusHistoryAggregate>> ExecuteAsync(
        CancellationToken cancellationToken = default)
        => await _repository.GetAllAsync(cancellationToken);
}
