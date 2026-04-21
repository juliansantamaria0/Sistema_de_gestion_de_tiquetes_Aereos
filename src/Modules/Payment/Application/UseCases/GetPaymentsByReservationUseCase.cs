namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Domain.Repositories;

/// <summary>Obtiene todos los pagos asociados a una reserva.</summary>
public sealed class GetPaymentsByReservationUseCase
{
    private readonly IPaymentRepository _repository;

    public GetPaymentsByReservationUseCase(IPaymentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<PaymentAggregate>> ExecuteAsync(
        int               reservationId,
        CancellationToken cancellationToken = default)
        => await _repository.GetByReservationAsync(reservationId, cancellationToken);
}
