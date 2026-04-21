namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatus.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatus.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatus.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class UpdateReservationStatusUseCase
{
    private readonly IReservationStatusRepository _repository;
    private readonly IUnitOfWork                  _unitOfWork;

    public UpdateReservationStatusUseCase(IReservationStatusRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int               id,
        string            newName,
        CancellationToken cancellationToken = default)
    {
        var reservationStatus = await _repository.GetByIdAsync(new ReservationStatusId(id), cancellationToken)
            ?? throw new KeyNotFoundException($"ReservationStatus with id {id} was not found.");

        reservationStatus.UpdateName(newName);
        await _repository.UpdateAsync(reservationStatus, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
