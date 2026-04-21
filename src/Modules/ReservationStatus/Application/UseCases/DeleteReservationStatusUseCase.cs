namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatus.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatus.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatus.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class DeleteReservationStatusUseCase
{
    private readonly IReservationStatusRepository _repository;
    private readonly IUnitOfWork                  _unitOfWork;

    public DeleteReservationStatusUseCase(IReservationStatusRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(new ReservationStatusId(id), cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
