namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Domain.Repositories;





public sealed class GetReservationDetailsByReservationUseCase
{
    private readonly IReservationDetailRepository _repository;

    public GetReservationDetailsByReservationUseCase(IReservationDetailRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ReservationDetailAggregate>> ExecuteAsync(
        int               reservationId,
        CancellationToken cancellationToken = default)
        => await _repository.GetByReservationAsync(reservationId, cancellationToken);
}
