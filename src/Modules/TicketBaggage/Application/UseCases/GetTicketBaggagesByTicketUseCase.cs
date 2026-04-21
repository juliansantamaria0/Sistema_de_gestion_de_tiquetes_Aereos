namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.Domain.Repositories;

/// <summary>
/// Obtiene todo el equipaje adicional registrado en un tiquete.
/// Caso de uso clave para mostrar el detalle completo del tiquete
/// y calcular el costo total de equipaje adicional.
/// </summary>
public sealed class GetTicketBaggagesByTicketUseCase
{
    private readonly ITicketBaggageRepository _repository;

    public GetTicketBaggagesByTicketUseCase(ITicketBaggageRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TicketBaggageAggregate>> ExecuteAsync(
        int               ticketId,
        CancellationToken cancellationToken = default)
        => await _repository.GetByTicketAsync(ticketId, cancellationToken);
}
