namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Application.UseCases;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreatePersonUseCase
{
    private readonly IPersonRepository _repository;
    private readonly IUnitOfWork       _unitOfWork;
    private readonly AppDbContext      _context;

    public CreatePersonUseCase(IPersonRepository repository, IUnitOfWork unitOfWork, AppDbContext context)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _context    = context;
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
        var normalizedDocument = documentNumber.Trim().ToUpperInvariant();

        if (!await _context.DocumentTypes.AsNoTracking().AnyAsync(x => x.DocumentTypeId == documentTypeId, cancellationToken))
            throw new InvalidOperationException($"No existe el tipo de documento con id {documentTypeId}.");

        if (genderId.HasValue && !await _context.Genders.AsNoTracking().AnyAsync(x => x.Id == genderId.Value, cancellationToken))
            throw new InvalidOperationException($"No existe el género con id {genderId.Value}.");

        if (string.IsNullOrWhiteSpace(normalizedDocument))
            throw new InvalidOperationException("El número de documento es obligatorio.");

        if (await _context.Persons.AsNoTracking().AnyAsync(
                x => x.DocumentTypeId == documentTypeId && x.DocumentNumber == normalizedDocument,
                cancellationToken))
            throw new InvalidOperationException("Ya existe una persona con ese tipo y número de documento.");

        var person = new PersonAggregate(
            new PersonId(0),
            documentTypeId, normalizedDocument,
            firstName, lastName,
            birthDate, genderId,
            now);

        await _repository.AddAsync(person, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return person;
    }
}
