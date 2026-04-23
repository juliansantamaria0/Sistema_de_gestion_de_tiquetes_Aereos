namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.CancellationReason.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CancellationReason.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CancellationReason.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CancellationReason.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreateCancellationReasonUseCase
{
    private readonly ICancellationReasonRepository _repository;
    private readonly IUnitOfWork                   _unitOfWork;

    public CreateCancellationReasonUseCase(ICancellationReasonRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CancellationReasonAggregate> ExecuteAsync(
        string            name,
        CancellationToken cancellationToken = default)
    {
        
        var cancellationReason = new CancellationReasonAggregate(new CancellationReasonId(0), name);

        await _repository.AddAsync(cancellationReason, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return cancellationReason;
    }
}
