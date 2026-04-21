namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class DeletePersonUseCase
{
    private readonly IPersonRepository _repository;
    private readonly IUnitOfWork       _unitOfWork;

    public DeletePersonUseCase(IPersonRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(new PersonId(id), cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
