namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerContact.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerContact.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerContact.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerContact.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreatePassengerContactUseCase
{
    private readonly IPassengerContactRepository _repository;
    private readonly IUnitOfWork                 _unitOfWork;

    public CreatePassengerContactUseCase(
        IPassengerContactRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PassengerContactAggregate> ExecuteAsync(
        int               passengerId,
        int               contactTypeId,
        string            fullName,
        string            phone,
        string?           relationship,
        CancellationToken cancellationToken = default)
    {
        var contact = new PassengerContactAggregate(
            new PassengerContactId(await GetNextIdAsync(cancellationToken)),
            passengerId, contactTypeId, fullName, phone, relationship);

        await _repository.AddAsync(contact, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return contact;
    }

    private async Task<int> GetNextIdAsync(CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return items.Select(x => x.Id.Value).DefaultIfEmpty(0).Max() + 1;
    }
}
