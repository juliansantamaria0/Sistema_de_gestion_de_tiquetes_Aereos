namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Domain.ValueObject;

public sealed class GetReservationDetailByIdUseCase
{
    private readonly IReservationDetailRepository _repository;

    public GetReservationDetailByIdUseCase(IReservationDetailRepository repository)
    {
        _repository = repository;
    }

    public async Task<ReservationDetailAggregate?> ExecuteAsync(
        int               id,
        CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(new ReservationDetailId(id), cancellationToken);
}
