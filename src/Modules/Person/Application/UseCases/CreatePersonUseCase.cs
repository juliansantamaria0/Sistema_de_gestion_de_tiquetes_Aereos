namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreatePersonUseCase
{
    private readonly IPersonRepository _repository;
    private readonly IUnitOfWork       _unitOfWork;

    public CreatePersonUseCase(IPersonRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PersonAggregate> ExecuteAsync(
        int               documentTypeId,
        string            documentNumber,
        string            firstName,
        string            lastName,
        DateOnly?         birthDate,
        int?              genderId,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var person = new PersonAggregate(
            new PersonId(1),
            documentTypeId, documentNumber,
            firstName, lastName,
            birthDate, genderId,
            now);

        await _repository.AddAsync(person, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return person;
    }
}
