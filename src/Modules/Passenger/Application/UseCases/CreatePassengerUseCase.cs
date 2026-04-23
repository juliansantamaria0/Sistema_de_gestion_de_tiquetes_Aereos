namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Application.UseCases;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreatePassengerUseCase
{
    private readonly IPassengerRepository _repository;
    private readonly IUnitOfWork          _unitOfWork;
    private readonly AppDbContext         _context;

    public CreatePassengerUseCase(IPassengerRepository repository, IUnitOfWork unitOfWork, AppDbContext context)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _context    = context;
    }

    public async Task<PassengerAggregate> ExecuteAsync(
        int               personId,
        string?           frequentFlyerNumber,
        int?              nationalityId,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var normalizedFfn = string.IsNullOrWhiteSpace(frequentFlyerNumber)
            ? null
            : frequentFlyerNumber.Trim().ToUpperInvariant();

        if (!await _context.Persons.AsNoTracking().AnyAsync(x => x.Id == personId, cancellationToken))
            throw new InvalidOperationException($"No existe la persona con id {personId}.");

        if (await _context.Passengers.AsNoTracking().AnyAsync(x => x.PersonId == personId, cancellationToken))
            throw new InvalidOperationException("Ya existe un pasajero asociado a esta persona.");

        if (nationalityId.HasValue && !await _context.Nationalities.AsNoTracking().AnyAsync(x => x.NationalityId == nationalityId.Value, cancellationToken))
            throw new InvalidOperationException($"No existe la nacionalidad con id {nationalityId.Value}.");

        if (normalizedFfn is not null &&
            await _context.Passengers.AsNoTracking().AnyAsync(x => x.FrequentFlyerNumber == normalizedFfn, cancellationToken))
            throw new InvalidOperationException("Ya existe un pasajero con ese número de viajero frecuente.");

        var passenger = new PassengerAggregate(
            new PassengerId(0), personId, normalizedFfn, nationalityId, now);

        await _repository.AddAsync(passenger, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return passenger;
    }
}
