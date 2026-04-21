namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentStatus.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentStatus.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentStatus.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class UpdatePaymentStatusUseCase
{
    private readonly IPaymentStatusRepository _repository;
    private readonly IUnitOfWork              _unitOfWork;

    public UpdatePaymentStatusUseCase(IPaymentStatusRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int               id,
        string            newName,
        CancellationToken cancellationToken = default)
    {
        var paymentStatus = await _repository.GetByIdAsync(new PaymentStatusId(id), cancellationToken)
            ?? throw new KeyNotFoundException($"PaymentStatus with id {id} was not found.");

        paymentStatus.UpdateName(newName);
        await _repository.UpdateAsync(paymentStatus, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
