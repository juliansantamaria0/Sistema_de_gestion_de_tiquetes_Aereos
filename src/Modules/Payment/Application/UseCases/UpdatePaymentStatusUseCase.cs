namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;






public sealed class UpdatePaymentStatusUseCase
{
    private readonly IPaymentRepository _repository;
    private readonly IUnitOfWork        _unitOfWork;

    public UpdatePaymentStatusUseCase(IPaymentRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int               id,
        int               paymentStatusId,
        string?           transactionReference,
        string?           rejectionReason,
        CancellationToken cancellationToken = default)
    {
        var payment = await _repository.GetByIdAsync(new PaymentId(id), cancellationToken)
            ?? throw new KeyNotFoundException($"Payment with id {id} was not found.");

        payment.UpdateStatus(paymentStatusId, transactionReference, rejectionReason);
        await _repository.UpdateAsync(payment, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
