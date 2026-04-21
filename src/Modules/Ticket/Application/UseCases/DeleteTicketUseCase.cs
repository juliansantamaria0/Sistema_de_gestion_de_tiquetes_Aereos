namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class DeleteTicketUseCase
{
    private readonly ITicketRepository _repository;
    private readonly IUnitOfWork       _unitOfWork;

    public DeleteTicketUseCase(ITicketRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(new TicketId(id), cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
