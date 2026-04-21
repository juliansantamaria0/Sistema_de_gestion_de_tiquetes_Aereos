namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gender.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gender.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gender.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class UpdateGenderUseCase
{
    private readonly IGenderRepository _repository;
    private readonly IUnitOfWork       _unitOfWork;

    public UpdateGenderUseCase(IGenderRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int               id,
        string            newName,
        CancellationToken cancellationToken = default)
    {
        var gender = await _repository.GetByIdAsync(new GenderId(id), cancellationToken)
            ?? throw new KeyNotFoundException($"Gender with id {id} was not found.");

        gender.UpdateName(newName);
        await _repository.UpdateAsync(gender, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
