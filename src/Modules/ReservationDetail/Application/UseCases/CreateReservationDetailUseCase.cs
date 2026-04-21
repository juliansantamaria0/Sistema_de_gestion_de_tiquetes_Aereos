namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreateReservationDetailUseCase
{
    private readonly IReservationDetailRepository _repository;
    private readonly IUnitOfWork                  _unitOfWork;

    public CreateReservationDetailUseCase(IReservationDetailRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ReservationDetailAggregate> ExecuteAsync(
        int               reservationId,
        int               passengerId,
        int               flightSeatId,
        int               fareTypeId,
        CancellationToken cancellationToken = default)
    {
        // ReservationDetailId(1) es placeholder; EF Core asigna el Id real al insertar.
        // NOTA: el trigger RF-6 de la BD verifica que el asiento esté AVAILABLE.
        var detail = new ReservationDetailAggregate(
            new ReservationDetailId(1),
            reservationId,
            passengerId,
            flightSeatId,
            fareTypeId,
            DateTime.UtcNow);

        await _repository.AddAsync(detail, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return detail;
    }
}
