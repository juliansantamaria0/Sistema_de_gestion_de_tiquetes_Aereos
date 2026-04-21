namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentMethod.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentMethod.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentMethod.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class DeletePaymentMethodUseCase
{
    private readonly IPaymentMethodRepository _repository;
    private readonly IUnitOfWork              _unitOfWork;

    public DeletePaymentMethodUseCase(IPaymentMethodRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(new PaymentMethodId(id), cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
