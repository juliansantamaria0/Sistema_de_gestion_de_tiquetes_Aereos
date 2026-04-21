namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatus.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatus.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatus.Domain.Repositories;

public sealed class GetAllTicketStatusesUseCase
{
    private readonly ITicketStatusRepository _repository;

    public GetAllTicketStatusesUseCase(ITicketStatusRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TicketStatusAggregate>> ExecuteAsync(
        CancellationToken cancellationToken = default)
        => await _repository.GetAllAsync(cancellationToken);
}
