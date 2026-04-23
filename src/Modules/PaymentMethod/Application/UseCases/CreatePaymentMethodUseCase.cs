namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentMethod.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentMethod.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentMethod.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentMethod.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreatePaymentMethodUseCase
{
    private readonly IPaymentMethodRepository _repository;
    private readonly IUnitOfWork              _unitOfWork;

    public CreatePaymentMethodUseCase(IPaymentMethodRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PaymentMethodAggregate> ExecuteAsync(
        string            name,
        CancellationToken cancellationToken = default)
    {
        // PaymentMethodId(1) es placeholder; EF Core asigna el Id real al insertar.
        var paymentMethod = new PaymentMethodAggregate(new PaymentMethodId(await GetNextIdAsync(cancellationToken)), name);

        await _repository.AddAsync(paymentMethod, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return paymentMethod;
    }

    private async Task<int> GetNextIdAsync(CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return items.Select(x => x.Id.Value).DefaultIfEmpty(0).Max() + 1;
    }
}
