namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class RecordTicketStatusHistoryUseCase
{
    private readonly ITicketStatusHistoryRepository _repository;
    private readonly IUnitOfWork                    _unitOfWork;

    public RecordTicketStatusHistoryUseCase(
        ITicketStatusHistoryRepository repository,
        IUnitOfWork                    unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TicketStatusHistoryAggregate> ExecuteAsync(
        int               ticketId,
        int               ticketStatusId,
        string?           notes,
        CancellationToken cancellationToken = default)
    {
        var entry = new TicketStatusHistoryAggregate(
            new TicketStatusHistoryId(1),
            ticketId, ticketStatusId, DateTime.UtcNow, notes);

        await _repository.AddAsync(entry, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return entry;
    }
}
