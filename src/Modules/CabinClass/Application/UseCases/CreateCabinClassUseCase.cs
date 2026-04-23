namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.CabinClass.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CabinClass.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CabinClass.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CabinClass.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreateCabinClassUseCase
{
    private readonly ICabinClassRepository _repository;
    private readonly IUnitOfWork           _unitOfWork;

    public CreateCabinClassUseCase(ICabinClassRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CabinClassAggregate> ExecuteAsync(
        string name, CancellationToken cancellationToken = default)
    {
        var cabinClass = new CabinClassAggregate(new CabinClassId(await GetNextIdAsync(cancellationToken)), name);
        await _repository.AddAsync(cabinClass, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return cabinClass;
    }

    private async Task<int> GetNextIdAsync(CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return items.Select(x => x.Id.Value).DefaultIfEmpty(0).Max() + 1;
    }
}
