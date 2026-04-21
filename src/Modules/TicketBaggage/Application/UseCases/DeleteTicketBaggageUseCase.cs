namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class DeleteTicketBaggageUseCase
{
    private readonly ITicketBaggageRepository _repository;
    private readonly IUnitOfWork              _unitOfWork;

    public DeleteTicketBaggageUseCase(ITicketBaggageRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(new TicketBaggageId(id), cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
