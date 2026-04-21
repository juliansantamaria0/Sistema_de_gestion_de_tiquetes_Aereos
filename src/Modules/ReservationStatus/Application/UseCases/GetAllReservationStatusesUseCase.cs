namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatus.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatus.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatus.Domain.Repositories;

public sealed class GetAllReservationStatusesUseCase
{
    private readonly IReservationStatusRepository _repository;

    public GetAllReservationStatusesUseCase(IReservationStatusRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ReservationStatusAggregate>> ExecuteAsync(
        CancellationToken cancellationToken = default)
        => await _repository.GetAllAsync(cancellationToken);
}
