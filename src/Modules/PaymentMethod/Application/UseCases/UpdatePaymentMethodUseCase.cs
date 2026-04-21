namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentMethod.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentMethod.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentMethod.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class UpdatePaymentMethodUseCase
{
    private readonly IPaymentMethodRepository _repository;
    private readonly IUnitOfWork              _unitOfWork;

    public UpdatePaymentMethodUseCase(IPaymentMethodRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int               id,
        string            newName,
        CancellationToken cancellationToken = default)
    {
        var paymentMethod = await _repository.GetByIdAsync(new PaymentMethodId(id), cancellationToken)
            ?? throw new KeyNotFoundException($"PaymentMethod with id {id} was not found.");

        paymentMethod.UpdateName(newName);
        await _repository.UpdateAsync(paymentMethod, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
