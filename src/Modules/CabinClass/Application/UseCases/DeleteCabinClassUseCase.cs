namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.CabinClass.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CabinClass.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CabinClass.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class DeleteCabinClassUseCase
{
    private readonly ICabinClassRepository _repository;
    private readonly IUnitOfWork           _unitOfWork;

    public DeleteCabinClassUseCase(ICabinClassRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(new CabinClassId(id), cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
