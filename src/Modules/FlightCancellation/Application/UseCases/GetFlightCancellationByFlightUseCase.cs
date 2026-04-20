namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Domain.Repositories;

/// <summary>
/// Obtiene el registro de cancelación de un vuelo programado dado.
/// Retorna null si el vuelo no ha sido cancelado.
/// Caso de uso clave para verificar si un vuelo está cancelado antes de
/// procesar reservas o check-in.
/// </summary>
public sealed class GetFlightCancellationByFlightUseCase
{
    private readonly IFlightCancellationRepository _repository;

    public GetFlightCancellationByFlightUseCase(IFlightCancellationRepository repository)
    {
        _repository = repository;
    }

    public async Task<FlightCancellationAggregate?> ExecuteAsync(
        int               scheduledFlightId,
        CancellationToken cancellationToken = default)
        => await _repository.GetByFlightAsync(scheduledFlightId, cancellationToken);
}
