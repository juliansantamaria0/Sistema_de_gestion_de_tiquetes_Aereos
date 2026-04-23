namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerContact.Infrastructure.Repository;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerContact.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerContact.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerContact.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerContact.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class PassengerContactRepository : IPassengerContactRepository
{
    private readonly AppDbContext _context;

    public PassengerContactRepository(AppDbContext context) => _context = context;

    private static PassengerContactAggregate ToDomain(PassengerContactEntity e)
        => new(new PassengerContactId(e.Id),
               e.PassengerId, e.ContactTypeId, e.FullName, e.Phone, e.Relationship);

    public async Task<PassengerContactAggregate?> GetByIdAsync(
        PassengerContactId id, CancellationToken ct = default)
    {
        var e = await _context.PassengerContacts.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id.Value, ct);
        return e is null ? null : ToDomain(e);
    }

    public async Task<IEnumerable<PassengerContactAggregate>> GetAllAsync(
        CancellationToken ct = default)
    {
        var entities = await _context.PassengerContacts.AsNoTracking()
            .OrderBy(x => x.PassengerId).ThenBy(x => x.ContactTypeId).ToListAsync(ct);
        return entities.Select(ToDomain);
    }

    public async Task<IEnumerable<PassengerContactAggregate>> GetByPassengerAsync(
        int passengerId, CancellationToken ct = default)
    {
        var entities = await _context.PassengerContacts.AsNoTracking()
            .Where(x => x.PassengerId == passengerId).OrderBy(x => x.ContactTypeId).ToListAsync(ct);
        return entities.Select(ToDomain);
    }

    public async Task AddAsync(PassengerContactAggregate contact, CancellationToken ct = default)
    {
        await _context.PassengerContacts.AddAsync(new PassengerContactEntity
        {
            PassengerId   = contact.PassengerId,
            ContactTypeId = contact.ContactTypeId,
            FullName      = contact.FullName,
            Phone         = contact.Phone,
            Relationship  = contact.Relationship
        }, ct);
    }

    public async Task UpdateAsync(PassengerContactAggregate contact, CancellationToken ct = default)
    {
        var e = await _context.PassengerContacts
            .FirstOrDefaultAsync(x => x.Id == contact.Id.Value, ct)
            ?? throw new KeyNotFoundException(
                $"PassengerContactEntity with id {contact.Id.Value} not found.");

        
        e.FullName     = contact.FullName;
        e.Phone        = contact.Phone;
        e.Relationship = contact.Relationship;
        _context.PassengerContacts.Update(e);
    }

    public async Task DeleteAsync(PassengerContactId id, CancellationToken ct = default)
    {
        var e = await _context.PassengerContacts
            .FirstOrDefaultAsync(x => x.Id == id.Value, ct)
            ?? throw new KeyNotFoundException(
                $"PassengerContactEntity with id {id.Value} not found.");
        _context.PassengerContacts.Remove(e);
    }
}
