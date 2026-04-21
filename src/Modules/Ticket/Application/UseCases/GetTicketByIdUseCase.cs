namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Domain.ValueObject;

public sealed class GetTicketByIdUseCase
{
    private readonly ITicketRepository _repository;

    public GetTicketByIdUseCase(ITicketRepository repository)
    {
        _repository = repository;
    }

    public async Task<TicketAggregate?> ExecuteAsync(
        int               id,
        CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(new TicketId(id), cancellationToken);
}
