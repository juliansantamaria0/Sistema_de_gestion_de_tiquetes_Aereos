namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Domain.Repositories;

public sealed class GetTicketStatusHistoryByTicketUseCase
{
    private readonly ITicketStatusHistoryRepository _repository;

    public GetTicketStatusHistoryByTicketUseCase(ITicketStatusHistoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TicketStatusHistoryAggregate>> ExecuteAsync(
        int               ticketId,
        CancellationToken cancellationToken = default)
        => await _repository.GetByTicketAsync(ticketId, cancellationToken);
}
