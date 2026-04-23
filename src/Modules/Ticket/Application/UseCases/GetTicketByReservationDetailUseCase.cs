namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Domain.Repositories;






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
