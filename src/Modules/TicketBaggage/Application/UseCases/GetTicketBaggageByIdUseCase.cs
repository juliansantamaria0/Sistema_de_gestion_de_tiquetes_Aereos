namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.Domain.ValueObject;

public sealed class GetTicketBaggageByIdUseCase
{
    private readonly ITicketBaggageRepository _repository;

    public GetTicketBaggageByIdUseCase(ITicketBaggageRepository repository)
    {
        _repository = repository;
    }

    public async Task<TicketBaggageAggregate?> ExecuteAsync(
        int               id,
        CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(new TicketBaggageId(id), cancellationToken);
}
