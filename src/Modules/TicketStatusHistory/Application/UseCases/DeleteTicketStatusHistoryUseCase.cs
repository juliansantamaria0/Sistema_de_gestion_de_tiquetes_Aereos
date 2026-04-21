namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class DeleteTicketStatusHistoryUseCase
{
    private readonly ITicketStatusHistoryRepository _repository;
    private readonly IUnitOfWork                    _unitOfWork;

    public DeleteTicketStatusHistoryUseCase(
        ITicketStatusHistoryRepository repository,
        IUnitOfWork                    unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(new TicketStatusHistoryId(id), cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
