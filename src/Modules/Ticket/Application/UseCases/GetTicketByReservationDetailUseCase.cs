namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Domain.Repositories;

/// <summary>
/// Obtiene el tiquete de una línea de reserva.
/// Retorna null si aún no se ha emitido el tiquete para esa línea.
/// La UNIQUE sobre reservation_detail_id garantiza como máximo un resultado.
/// </summary>
public sealed class GetTicketByReservationDetailUseCase
{
    private readonly ITicketRepository _repository;

    public GetTicketByReservationDetailUseCase(ITicketRepository repository)
    {
        _repository = repository;
    }

    public async Task<TicketAggregate?> ExecuteAsync(
        int               reservationDetailId,
        CancellationToken cancellationToken = default)
        => await _repository.GetByReservationDetailAsync(reservationDetailId, cancellationToken);
}
