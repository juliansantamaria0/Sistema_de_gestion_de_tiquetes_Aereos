namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatus.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatus.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatus.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatus.Domain.ValueObject;

public sealed class GetReservationStatusByIdUseCase
{
    private readonly IReservationStatusRepository _repository;

    public GetReservationStatusByIdUseCase(IReservationStatusRepository repository)
    {
        _repository = repository;
    }

    public async Task<ReservationStatusAggregate?> ExecuteAsync(
        int               id,
        CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(new ReservationStatusId(id), cancellationToken);
}
