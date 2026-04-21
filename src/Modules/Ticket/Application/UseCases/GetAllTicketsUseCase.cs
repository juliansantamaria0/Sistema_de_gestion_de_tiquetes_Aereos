namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Domain.Repositories;

public sealed class GetAllTicketsUseCase
{
    private readonly ITicketRepository _repository;

    public GetAllTicketsUseCase(ITicketRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TicketAggregate>> ExecuteAsync(
        CancellationToken cancellationToken = default)
        => await _repository.GetAllAsync(cancellationToken);
}
