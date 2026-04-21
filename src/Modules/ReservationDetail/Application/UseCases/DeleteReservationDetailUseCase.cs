namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class DeleteReservationDetailUseCase
{
    private readonly IReservationDetailRepository _repository;
    private readonly IUnitOfWork                  _unitOfWork;

    public DeleteReservationDetailUseCase(IReservationDetailRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(new ReservationDetailId(id), cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
