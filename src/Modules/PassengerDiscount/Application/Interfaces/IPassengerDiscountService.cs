namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Application.Interfaces;

public interface IPassengerDiscountService
{
    Task<PassengerDiscountDto?>             GetByIdAsync(int id,                                                                   CancellationToken cancellationToken = default);
    Task<IEnumerable<PassengerDiscountDto>> GetAllAsync(                                                                           CancellationToken cancellationToken = default);
    Task<IEnumerable<PassengerDiscountDto>> GetByReservationDetailAsync(int reservationDetailId,                                   CancellationToken cancellationToken = default);
    Task<PassengerDiscountDto>              CreateAsync(int reservationDetailId, int discountTypeId, decimal amountApplied,        CancellationToken cancellationToken = default);
    Task                                    AdjustAmountAsync(int id, decimal newAmount,                                           CancellationToken cancellationToken = default);
    Task                                    DeleteAsync(int id,                                                                    CancellationToken cancellationToken = default);
}

public sealed record PassengerDiscountDto(
    int     Id,
    int     ReservationDetailId,
    int     DiscountTypeId,
    decimal AmountApplied);
