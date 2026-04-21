namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Domain.Repositories;

public sealed class GetAllReservationDetailsUseCase
{
    private readonly IReservationDetailRepository _repository;

    public GetAllReservationDetailsUseCase(IReservationDetailRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ReservationDetailAggregate>> ExecuteAsync(
        CancellationToken cancellationToken = default)
        => await _repository.GetAllAsync(cancellationToken);
}
