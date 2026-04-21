namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.ValueObject;

public sealed class GetReservationByIdUseCase
{
    private readonly IReservationRepository _repository;

    public GetReservationByIdUseCase(IReservationRepository repository)
    {
        _repository = repository;
    }

    public async Task<ReservationAggregate?> ExecuteAsync(
        int               id,
        CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(new ReservationId(id), cancellationToken);
}
