namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

/// <summary>
/// Cambia la tarifa seleccionada por el pasajero en esta línea de reserva.
/// ReservationId, PassengerId y FlightSeatId son la clave de negocio — inmutables.
/// </summary>
public sealed class UpdateReservationDetailUseCase
{
    private readonly IReservationDetailRepository _repository;
    private readonly IUnitOfWork                  _unitOfWork;

    public UpdateReservationDetailUseCase(IReservationDetailRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int               id,
        int               fareTypeId,
        CancellationToken cancellationToken = default)
    {
        var detail = await _repository.GetByIdAsync(new ReservationDetailId(id), cancellationToken)
            ?? throw new KeyNotFoundException($"ReservationDetail with id {id} was not found.");

        detail.ChangeFareType(fareTypeId);
        await _repository.UpdateAsync(detail, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
