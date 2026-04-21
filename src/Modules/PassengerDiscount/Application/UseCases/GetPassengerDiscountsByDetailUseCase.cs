namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Domain.Repositories;

/// <summary>
/// Obtiene todos los descuentos aplicados a una línea de reserva concreta.
/// Caso de uso clave para calcular el precio neto de un pasajero.
/// </summary>
public sealed class GetPassengerDiscountsByDetailUseCase
{
    private readonly IPassengerDiscountRepository _repository;

    public GetPassengerDiscountsByDetailUseCase(IPassengerDiscountRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<PassengerDiscountAggregate>> ExecuteAsync(
        int               reservationDetailId,
        CancellationToken cancellationToken = default)
        => await _repository.GetByReservationDetailAsync(reservationDetailId, cancellationToken);
}
