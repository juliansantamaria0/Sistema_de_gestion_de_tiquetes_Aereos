namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.RefundStatus.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RefundStatus.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RefundStatus.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class DeleteRefundStatusUseCase
{
    private readonly IRefundStatusRepository _repository;
    private readonly IUnitOfWork             _unitOfWork;

    public DeleteRefundStatusUseCase(IRefundStatusRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(new RefundStatusId(id), cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
