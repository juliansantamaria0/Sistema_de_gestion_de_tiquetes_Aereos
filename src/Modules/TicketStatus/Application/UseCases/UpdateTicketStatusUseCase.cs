namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatus.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatus.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatus.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class UpdateTicketStatusUseCase
{
    private readonly ITicketStatusRepository _repository;
    private readonly IUnitOfWork             _unitOfWork;

    public UpdateTicketStatusUseCase(ITicketStatusRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int               id,
        string            newName,
        CancellationToken cancellationToken = default)
    {
        var ticketStatus = await _repository.GetByIdAsync(new TicketStatusId(id), cancellationToken)
            ?? throw new KeyNotFoundException($"TicketStatus with id {id} was not found.");

        ticketStatus.UpdateName(newName);
        await _repository.UpdateAsync(ticketStatus, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
