namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Domain.Aggregate;

public sealed class PassengerDiscountService : IPassengerDiscountService
{
    private readonly CreatePassengerDiscountUseCase           _create;
    private readonly DeletePassengerDiscountUseCase           _delete;
    private readonly GetAllPassengerDiscountsUseCase          _getAll;
    private readonly GetPassengerDiscountByIdUseCase          _getById;
    private readonly UpdatePassengerDiscountUseCase           _update;
    private readonly GetPassengerDiscountsByDetailUseCase     _getByDetail;

    public PassengerDiscountService(
        CreatePassengerDiscountUseCase       create,
        DeletePassengerDiscountUseCase       delete,
        GetAllPassengerDiscountsUseCase      getAll,
        GetPassengerDiscountByIdUseCase      getById,
        UpdatePassengerDiscountUseCase       update,
        GetPassengerDiscountsByDetailUseCase getByDetail)
    {
        _create      = create;
        _delete      = delete;
        _getAll      = getAll;
        _getById     = getById;
        _update      = update;
        _getByDetail = getByDetail;
    }

    public async Task<PassengerDiscountDto> CreateAsync(
        int               reservationDetailId,
        int               discountTypeId,
        decimal           amountApplied,
        CancellationToken cancellationToken = default)
    {
        var agg = await _create.ExecuteAsync(
            reservationDetailId, discountTypeId, amountApplied, cancellationToken);
        return ToDto(agg);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await _delete.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<PassengerDiscountDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var list = await _getAll.ExecuteAsync(cancellationToken);
        return list.Select(ToDto);
    }

    public async Task<PassengerDiscountDto?> GetByIdAsync(
        int               id,
        CancellationToken cancellationToken = default)
    {
        var agg = await _getById.ExecuteAsync(id, cancellationToken);
        return agg is null ? null : ToDto(agg);
    }

    public async Task AdjustAmountAsync(
        int               id,
        decimal           newAmount,
        CancellationToken cancellationToken = default)
        => await _update.ExecuteAsync(id, newAmount, cancellationToken);

    public async Task<IEnumerable<PassengerDiscountDto>> GetByReservationDetailAsync(
        int               reservationDetailId,
        CancellationToken cancellationToken = default)
    {
        var list = await _getByDetail.ExecuteAsync(reservationDetailId, cancellationToken);
        return list.Select(ToDto);
    }

    // ── Mapper privado ────────────────────────────────────────────────────────

    private static PassengerDiscountDto ToDto(PassengerDiscountAggregate agg)
        => new(agg.Id.Value, agg.ReservationDetailId, agg.DiscountTypeId, agg.AmountApplied);
}
