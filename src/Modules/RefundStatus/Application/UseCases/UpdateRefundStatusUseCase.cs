namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.RefundStatus.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RefundStatus.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RefundStatus.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class UpdateRefundStatusUseCase
{
    private readonly IRefundStatusRepository _repository;
    private readonly IUnitOfWork             _unitOfWork;

    public UpdateRefundStatusUseCase(IRefundStatusRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int               id,
        string            newName,
        CancellationToken cancellationToken = default)
    {
        var refundStatus = await _repository.GetByIdAsync(new RefundStatusId(id), cancellationToken)
            ?? throw new KeyNotFoundException($"RefundStatus with id {id} was not found.");

        refundStatus.UpdateName(newName);
        await _repository.UpdateAsync(refundStatus, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
