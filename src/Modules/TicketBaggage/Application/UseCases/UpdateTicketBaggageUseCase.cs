namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

/// <summary>
/// Ajusta la cantidad de piezas y la tarifa cobrada.
/// ticket_id y baggage_type_id son la clave de negocio — inmutables.
/// </summary>
public sealed class UpdateTicketBaggageUseCase
{
    private readonly ITicketBaggageRepository _repository;
    private readonly IUnitOfWork              _unitOfWork;

    public UpdateTicketBaggageUseCase(ITicketBaggageRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int               id,
        int               quantity,
        decimal           feeCharged,
        CancellationToken cancellationToken = default)
    {
        var ticketBaggage = await _repository.GetByIdAsync(new TicketBaggageId(id), cancellationToken)
            ?? throw new KeyNotFoundException($"TicketBaggage with id {id} was not found.");

        ticketBaggage.UpdateQuantityAndFee(quantity, feeCharged);
        await _repository.UpdateAsync(ticketBaggage, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
