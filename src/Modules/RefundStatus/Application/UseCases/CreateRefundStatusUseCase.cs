namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.RefundStatus.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RefundStatus.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RefundStatus.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RefundStatus.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreateRefundStatusUseCase
{
    private readonly IRefundStatusRepository _repository;
    private readonly IUnitOfWork             _unitOfWork;

    public CreateRefundStatusUseCase(IRefundStatusRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<RefundStatusAggregate> ExecuteAsync(
        string            name,
        CancellationToken cancellationToken = default)
    {
        
        var refundStatus = new RefundStatusAggregate(new RefundStatusId(0), name);

        await _repository.AddAsync(refundStatus, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return refundStatus;
    }
}
