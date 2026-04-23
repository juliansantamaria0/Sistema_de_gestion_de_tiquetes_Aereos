namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.Repositories;





public sealed class GetReservationsByCustomerUseCase
{
    private readonly IReservationRepository _repository;

    public GetReservationsByCustomerUseCase(IReservationRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ReservationAggregate>> ExecuteAsync(
        int               customerId,
        CancellationToken cancellationToken = default)
        => await _repository.GetByCustomerAsync(customerId, cancellationToken);
}
