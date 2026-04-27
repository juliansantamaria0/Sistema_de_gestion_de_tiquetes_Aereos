namespace Sistema_de_gestion_de_tiquetes_Aereos.Shared.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

public sealed class AuthService
{
    private readonly AppDbContext _context;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AuthResult> RegisterAsync(
        string username,
        string password,
        string firstName,
        string lastName,
        int documentTypeId,
        string documentNumber,
        string? email,
        string? phone,
        CancellationToken ct = default)
    {
        // Validar que el username no exista
        if (await _context.Users.AnyAsync(x => x.Username == username.ToLowerInvariant(), ct))
        {
            return AuthResult.Failed("El nombre de usuario ya está en uso.");
        }

        // Validar documento único
        if (await _context.Persons.AnyAsync(x => x.DocumentNumber == documentNumber && x.DocumentTypeId == documentTypeId, ct))
        {
            return AuthResult.Failed("Ya existe una persona registrada con este documento.");
        }

        var strategy = _context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(ct);
            try
            {
                var person = new Modules.Person.Infrastructure.Entity.PersonEntity
                {
                    FirstName = firstName,
                    LastName = lastName,
                    DocumentTypeId = documentTypeId,
                    DocumentNumber = documentNumber,
                    GenderId = 1,
                    BirthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-18)),
                    CreatedAt = DateTime.UtcNow
                };
                _context.Persons.Add(person);
                await _context.SaveChangesAsync(ct);

                var customer = new Modules.Customer.Infrastructure.Entity.CustomerEntity
                {
                    PersonId = person.Id,
                    Email = email,
                    Phone = phone,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync(ct);

                var passenger = new PassengerEntity
                {
                    PersonId = person.Id,
                    FrequentFlyerNumber = null,
                    NationalityId = null,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Passengers.Add(passenger);
                await _context.SaveChangesAsync(ct);

                var user = new Modules.User.Infrastructure.Entity.UserEntity
                {
                    PersonId = person.Id,
                    RoleId = 2,
                    Username = username.ToLowerInvariant(),
                    PasswordHash = PasswordHasher.Hash(password),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync(ct);

                await transaction.CommitAsync(ct);

                CurrentUser.Set(user.UserId, user.Username, person.Id, customer.Id);
                return AuthResult.Success(user.UserId, user.Username, person.Id, customer.Id);
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        });
    }

    public async Task<AuthResult> LoginAsync(string username, string password, CancellationToken ct = default)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Username == username.ToLowerInvariant(), ct);

        if (user is null)
        {
            return AuthResult.Failed("Usuario o contraseña incorrectos.");
        }

        if (!user.IsActive)
        {
            return AuthResult.Failed("La cuenta está desactivada.");
        }

        if (!PasswordHasher.Verify(password, user.PasswordHash))
        {
            return AuthResult.Failed("Usuario o contraseña incorrectos.");
        }

        // Obtener el cliente asociado
        var customer = await _context.Customers
            .FirstOrDefaultAsync(x => x.PersonId == user.PersonId, ct);

        if (customer is null)
        {
            return AuthResult.Failed("No hay un cliente asociado a este usuario.");
        }

        CurrentUser.Set(user.UserId, user.Username, user.PersonId, customer.Id);

        return AuthResult.Success(user.UserId, user.Username, user.PersonId, customer.Id);
    }

    public static void Logout()
    {
        CurrentUser.Clear();
    }
}

public sealed class AuthResult
{
    public bool IsSuccess { get; private set; }
    public string? ErrorMessage { get; private set; }
    public int? UserId { get; private set; }
    public string? Username { get; private set; }
    public int? PersonId { get; private set; }
    public int? CustomerId { get; private set; }

    private AuthResult() { }

    public static AuthResult Success(int userId, string username, int personId, int customerId) => new()
    {
        IsSuccess = true,
        UserId = userId,
        Username = username,
        PersonId = personId,
        CustomerId = customerId
    };

    public static AuthResult Failed(string errorMessage) => new()
    {
        IsSuccess = false,
        ErrorMessage = errorMessage
    };
}