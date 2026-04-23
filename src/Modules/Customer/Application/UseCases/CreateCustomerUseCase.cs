namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Customer.Application.UseCases;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Customer.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Customer.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Customer.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreateCustomerUseCase
{
    private readonly ICustomerRepository _repository;
    private readonly IUnitOfWork         _unitOfWork;
    private readonly AppDbContext        _context;

    public CreateCustomerUseCase(ICustomerRepository repository, IUnitOfWork unitOfWork, AppDbContext context)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _context    = context;
    }

    public async Task<CustomerAggregate> ExecuteAsync(
        int               personId,
        string?           phone,
        string?           email,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = string.IsNullOrWhiteSpace(email)
            ? null
            : email.Trim().ToLowerInvariant();

        if (!await _context.Persons.AsNoTracking().AnyAsync(x => x.Id == personId, cancellationToken))
            throw new InvalidOperationException($"No existe la persona con id {personId}.");

        if (await _context.Customers.AsNoTracking().AnyAsync(x => x.PersonId == personId, cancellationToken))
            throw new InvalidOperationException("Ya existe un cliente asociado a esta persona.");

        if (normalizedEmail is not null &&
            await _context.Customers.AsNoTracking().AnyAsync(x => x.Email != null && x.Email.ToLower() == normalizedEmail, cancellationToken))
            throw new InvalidOperationException("Ya existe un cliente con ese email.");

        
        var customer = new CustomerAggregate(
            new CustomerId(0),
            personId,
            phone,
            email,
            DateTime.UtcNow);

        await _repository.AddAsync(customer, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return customer;
    }
}
