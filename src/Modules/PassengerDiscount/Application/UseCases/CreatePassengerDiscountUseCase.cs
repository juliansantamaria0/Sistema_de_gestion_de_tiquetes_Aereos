namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreatePassengerDiscountUseCase
{
    private readonly IPassengerDiscountRepository _repository;
    private readonly IUnitOfWork                  _unitOfWork;

    public CreatePassengerDiscountUseCase(IPassengerDiscountRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PassengerDiscountAggregate> ExecuteAsync(
        int               reservationDetailId,
        int               discountTypeId,
        decimal           amountApplied,
        CancellationToken cancellationToken = default)
    {
        // PassengerDiscountId(1) es placeholder; EF Core asigna el Id real al insertar.
        var discount = new PassengerDiscountAggregate(
            new PassengerDiscountId(await GetNextIdAsync(cancellationToken)),
            reservationDetailId,
            discountTypeId,
            amountApplied);

        await _repository.AddAsync(discount, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return discount;
    }

    private async Task<int> GetNextIdAsync(CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return items.Select(x => x.Id.Value).DefaultIfEmpty(0).Max() + 1;
    }
}
