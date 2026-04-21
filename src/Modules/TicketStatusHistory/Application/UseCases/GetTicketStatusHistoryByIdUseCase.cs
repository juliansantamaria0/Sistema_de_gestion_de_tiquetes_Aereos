namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Domain.ValueObject;

public sealed class GetTicketStatusHistoryByIdUseCase
{
    private readonly ITicketStatusHistoryRepository _repository;

    public GetTicketStatusHistoryByIdUseCase(ITicketStatusHistoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<TicketStatusHistoryAggregate?> ExecuteAsync(
        int               id,
        CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(new TicketStatusHistoryId(id), cancellationToken);
}
