namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentStatus.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentStatus.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentStatus.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentStatus.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreatePaymentStatusUseCase
{
    private readonly IPaymentStatusRepository _repository;
    private readonly IUnitOfWork              _unitOfWork;

    public CreatePaymentStatusUseCase(IPaymentStatusRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PaymentStatusAggregate> ExecuteAsync(
        string            name,
        CancellationToken cancellationToken = default)
    {
        // PaymentStatusId(1) es placeholder; EF Core asigna el Id real al insertar.
        var paymentStatus = new PaymentStatusAggregate(new PaymentStatusId(1), name);

        await _repository.AddAsync(paymentStatus, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return paymentStatus;
    }
}
