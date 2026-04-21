namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.Domain.Repositories;

public sealed class GetAllTicketBaggagesUseCase
{
    private readonly ITicketBaggageRepository _repository;

    public GetAllTicketBaggagesUseCase(ITicketBaggageRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TicketBaggageAggregate>> ExecuteAsync(
        CancellationToken cancellationToken = default)
        => await _repository.GetAllAsync(cancellationToken);
}
