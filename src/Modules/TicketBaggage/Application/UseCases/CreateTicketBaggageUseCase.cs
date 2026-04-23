namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreateTicketBaggageUseCase
{
    private readonly ITicketBaggageRepository _repository;
    private readonly IUnitOfWork              _unitOfWork;

    public CreateTicketBaggageUseCase(ITicketBaggageRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TicketBaggageAggregate> ExecuteAsync(
        int               ticketId,
        int               baggageTypeId,
        int               quantity,
        decimal           feeCharged,
        CancellationToken cancellationToken = default)
    {
        
        var ticketBaggage = new TicketBaggageAggregate(
            new TicketBaggageId(0),
            ticketId, baggageTypeId, quantity, feeCharged);

        await _repository.AddAsync(ticketBaggage, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return ticketBaggage;
    }
}
