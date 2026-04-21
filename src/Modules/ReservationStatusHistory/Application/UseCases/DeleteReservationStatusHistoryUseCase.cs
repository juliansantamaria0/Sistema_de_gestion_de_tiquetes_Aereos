namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class DeleteReservationStatusHistoryUseCase
{
    private readonly IReservationStatusHistoryRepository _repository;
    private readonly IUnitOfWork                         _unitOfWork;

    public DeleteReservationStatusHistoryUseCase(
        IReservationStatusHistoryRepository repository,
        IUnitOfWork                         unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(new ReservationStatusHistoryId(id), cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
