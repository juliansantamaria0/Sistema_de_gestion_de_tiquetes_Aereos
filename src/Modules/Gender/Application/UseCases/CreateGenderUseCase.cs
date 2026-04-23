namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gender.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gender.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gender.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gender.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreateGenderUseCase
{
    private readonly IGenderRepository _repository;
    private readonly IUnitOfWork       _unitOfWork;

    public CreateGenderUseCase(IGenderRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<GenderAggregate> ExecuteAsync(
        string            name,
        CancellationToken cancellationToken = default)
    {
        var gender = new GenderAggregate(new GenderId(await GetNextIdAsync(cancellationToken)), name);
        await _repository.AddAsync(gender, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return gender;
    }

    private async Task<int> GetNextIdAsync(CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return items.Select(x => x.Id.Value).DefaultIfEmpty(0).Max() + 1;
    }
}
