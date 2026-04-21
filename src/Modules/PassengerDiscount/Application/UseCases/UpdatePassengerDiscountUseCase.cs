namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

/// <summary>
/// Ajusta el monto del descuento aplicado a un pasajero en una línea de reserva.
/// reservation_detail_id y discount_type_id son la clave de negocio — inmutables.
/// </summary>
public sealed class UpdatePassengerDiscountUseCase
{
    private readonly IPassengerDiscountRepository _repository;
    private readonly IUnitOfWork                  _unitOfWork;

    public UpdatePassengerDiscountUseCase(IPassengerDiscountRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int               id,
        decimal           newAmount,
        CancellationToken cancellationToken = default)
    {
        var discount = await _repository.GetByIdAsync(new PassengerDiscountId(id), cancellationToken)
            ?? throw new KeyNotFoundException($"PassengerDiscount with id {id} was not found.");

        discount.AdjustAmount(newAmount);
        await _repository.UpdateAsync(discount, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
