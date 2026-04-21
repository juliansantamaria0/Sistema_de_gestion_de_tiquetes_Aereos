namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ContactType.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ContactType.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ContactType.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ContactType.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ContactType.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class ContactTypeRepository : IContactTypeRepository
{
    private readonly AppDbContext _context;

    public ContactTypeRepository(AppDbContext context) => _context = context;

    private static ContactTypeAggregate ToDomain(ContactTypeEntity e)
        => new(new ContactTypeId(e.Id), e.Name);

    public async Task<ContactTypeAggregate?> GetByIdAsync(
        ContactTypeId id, CancellationToken ct = default)
    {
        var e = await _context.ContactTypes.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id.Value, ct);
        return e is null ? null : ToDomain(e);
    }

    public async Task<IEnumerable<ContactTypeAggregate>> GetAllAsync(
        CancellationToken ct = default)
    {
        var entities = await _context.ContactTypes.AsNoTracking()
            .OrderBy(x => x.Name).ToListAsync(ct);
        return entities.Select(ToDomain);
    }

    public async Task AddAsync(ContactTypeAggregate contactType, CancellationToken ct = default)
    {
        await _context.ContactTypes.AddAsync(
            new ContactTypeEntity { Name = contactType.Name }, ct);
    }

    public async Task UpdateAsync(ContactTypeAggregate contactType, CancellationToken ct = default)
    {
        var e = await _context.ContactTypes
            .FirstOrDefaultAsync(x => x.Id == contactType.Id.Value, ct)
            ?? throw new KeyNotFoundException(
                $"ContactTypeEntity with id {contactType.Id.Value} not found.");
        e.Name = contactType.Name;
        _context.ContactTypes.Update(e);
    }

    public async Task DeleteAsync(ContactTypeId id, CancellationToken ct = default)
    {
        var e = await _context.ContactTypes
            .FirstOrDefaultAsync(x => x.Id == id.Value, ct)
            ?? throw new KeyNotFoundException(
                $"ContactTypeEntity with id {id.Value} not found.");
        _context.ContactTypes.Remove(e);
    }
}
