namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class UpdatePersonUseCase
{
    private readonly IPersonRepository _repository;
    private readonly IUnitOfWork       _unitOfWork;

    public UpdatePersonUseCase(IPersonRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int               id,
        string            firstName,
        string            lastName,
        DateOnly?         birthDate,
        int?              genderId,
        CancellationToken cancellationToken = default)
    {
        var person = await _repository.GetByIdAsync(new PersonId(id), cancellationToken)
            ?? throw new KeyNotFoundException($"Person with id {id} was not found.");

        person.Update(firstName, lastName, birthDate, genderId);
        await _repository.UpdateAsync(person, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
