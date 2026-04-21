namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.Domain.ValueObject;

public sealed class GetReservationStatusHistoryByIdUseCase
{
    private readonly IReservationStatusHistoryRepository _repository;

    public GetReservationStatusHistoryByIdUseCase(IReservationStatusHistoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<ReservationStatusHistoryAggregate?> ExecuteAsync(
        int               id,
        CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(new ReservationStatusHistoryId(id), cancellationToken);
}
