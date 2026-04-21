namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatus.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatus.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatus.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatus.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreateTicketStatusUseCase
{
    private readonly ITicketStatusRepository _repository;
    private readonly IUnitOfWork             _unitOfWork;

    public CreateTicketStatusUseCase(ITicketStatusRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TicketStatusAggregate> ExecuteAsync(
        string            name,
        CancellationToken cancellationToken = default)
    {
        // TicketStatusId(1) es placeholder; EF Core asigna el Id real al insertar.
        var ticketStatus = new TicketStatusAggregate(new TicketStatusId(1), name);

        await _repository.AddAsync(ticketStatus, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return ticketStatus;
    }
}
