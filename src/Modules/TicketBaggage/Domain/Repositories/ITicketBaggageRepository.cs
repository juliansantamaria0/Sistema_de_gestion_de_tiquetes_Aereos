namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.Domain.Repositories;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.Domain.ValueObject;

public interface ITicketBaggageRepository
{
    Task<TicketBaggageAggregate?>             GetByIdAsync(TicketBaggageId id,                     CancellationToken cancellationToken = default);
    Task<IEnumerable<TicketBaggageAggregate>> GetAllAsync(                                           CancellationToken cancellationToken = default);
    Task<IEnumerable<TicketBaggageAggregate>> GetByTicketAsync(int ticketId,                         CancellationToken cancellationToken = default);
    Task                                      AddAsync(TicketBaggageAggregate ticketBaggage,         CancellationToken cancellationToken = default);
    Task                                      UpdateAsync(TicketBaggageAggregate ticketBaggage,      CancellationToken cancellationToken = default);
    Task                                      DeleteAsync(TicketBaggageId id,                        CancellationToken cancellationToken = default);
}
