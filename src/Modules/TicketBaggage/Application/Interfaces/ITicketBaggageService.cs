namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.Application.Interfaces;

public interface ITicketBaggageService
{
    Task<TicketBaggageDto?>             GetByIdAsync(int id,                                                              CancellationToken cancellationToken = default);
    Task<IEnumerable<TicketBaggageDto>> GetAllAsync(                                                                      CancellationToken cancellationToken = default);
    Task<IEnumerable<TicketBaggageDto>> GetByTicketAsync(int ticketId,                                                    CancellationToken cancellationToken = default);
    Task<TicketBaggageDto>              CreateAsync(int ticketId, int baggageTypeId, int quantity, decimal feeCharged,    CancellationToken cancellationToken = default);
    Task                                UpdateQuantityAndFeeAsync(int id, int quantity, decimal feeCharged,               CancellationToken cancellationToken = default);
    Task                                DeleteAsync(int id,                                                               CancellationToken cancellationToken = default);
}

public sealed record TicketBaggageDto(
    int     Id,
    int     TicketId,
    int     BaggageTypeId,
    int     Quantity,
    decimal FeeCharged);
