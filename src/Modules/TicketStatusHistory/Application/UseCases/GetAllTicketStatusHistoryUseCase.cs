namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Domain.Repositories;

public sealed class GetAllTicketStatusHistoryUseCase
{
    private readonly ITicketStatusHistoryRepository _repository;

    public GetAllTicketStatusHistoryUseCase(ITicketStatusHistoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TicketStatusHistoryAggregate>> ExecuteAsync(
        CancellationToken cancellationToken = default)
        => await _repository.GetAllAsync(cancellationToken);
}
