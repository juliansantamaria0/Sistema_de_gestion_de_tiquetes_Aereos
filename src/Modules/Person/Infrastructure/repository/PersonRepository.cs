namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class PersonRepository : IPersonRepository
{
    private readonly AppDbContext _context;

    public PersonRepository(AppDbContext context)
    {
        _context = context;
    }

    private static PersonAggregate ToDomain(PersonEntity entity)
        => new(
            new PersonId(entity.Id),
            entity.DocumentTypeId,
            entity.DocumentNumber,
            entity.FirstName,
            entity.LastName,
            entity.BirthDate,
            entity.GenderId,
            entity.CreatedAt,
            entity.UpdatedAt);

    public async Task<PersonAggregate?> GetByIdAsync(
        PersonId          id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Persons
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IEnumerable<PersonAggregate>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.Persons
            .AsNoTracking()
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain);
    }

    public async Task<PersonAggregate?> GetByDocumentAsync(
        int               documentTypeId,
        string            documentNumber,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Persons
            .AsNoTracking()
            .FirstOrDefaultAsync(
                e => e.DocumentTypeId == documentTypeId &&
                     e.DocumentNumber  == documentNumber.Trim(),
                cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task AddAsync(
        PersonAggregate   person,
        CancellationToken cancellationToken = default)
    {
        var entity = new PersonEntity
        {
            DocumentTypeId = person.DocumentTypeId,
            DocumentNumber = person.DocumentNumber,
            FirstName      = person.FirstName,
            LastName       = person.LastName,
            BirthDate      = person.BirthDate,
            GenderId       = person.GenderId,
            CreatedAt      = person.CreatedAt,
            UpdatedAt      = person.UpdatedAt
        };
        await _context.Persons.AddAsync(entity, cancellationToken);
    }

    public async Task UpdateAsync(
        PersonAggregate   person,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Persons
            .FirstOrDefaultAsync(e => e.Id == person.Id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"PersonEntity with id {person.Id.Value} not found.");

        
        entity.FirstName  = person.FirstName;
        entity.LastName   = person.LastName;
        entity.BirthDate  = person.BirthDate;
        entity.GenderId   = person.GenderId;
        entity.UpdatedAt  = person.UpdatedAt;

        _context.Persons.Update(entity);
    }

    public async Task DeleteAsync(
        PersonId          id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Persons
            .FirstOrDefaultAsync(e => e.Id == id.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"PersonEntity with id {id.Value} not found.");

        _context.Persons.Remove(entity);
    }
}
