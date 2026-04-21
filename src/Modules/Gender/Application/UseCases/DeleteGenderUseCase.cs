namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gender.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gender.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gender.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class DeleteGenderUseCase
{
    private readonly IGenderRepository _repository;
    private readonly IUnitOfWork       _unitOfWork;

    public DeleteGenderUseCase(IGenderRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(new GenderId(id), cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
