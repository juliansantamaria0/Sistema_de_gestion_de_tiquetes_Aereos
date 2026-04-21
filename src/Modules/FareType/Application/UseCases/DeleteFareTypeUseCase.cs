namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class DeleteFareTypeUseCase
{
    private readonly IFareTypeRepository _repository;
    private readonly IUnitOfWork         _unitOfWork;

    public DeleteFareTypeUseCase(IFareTypeRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(new FareTypeId(id), cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
