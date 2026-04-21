namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.CabinClass.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CabinClass.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CabinClass.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class UpdateCabinClassUseCase
{
    private readonly ICabinClassRepository _repository;
    private readonly IUnitOfWork           _unitOfWork;

    public UpdateCabinClassUseCase(ICabinClassRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int id, string newName, CancellationToken cancellationToken = default)
    {
        var cabinClass = await _repository.GetByIdAsync(new CabinClassId(id), cancellationToken)
            ?? throw new KeyNotFoundException($"CabinClass with id {id} was not found.");

        cabinClass.UpdateName(newName);
        await _repository.UpdateAsync(cabinClass, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
