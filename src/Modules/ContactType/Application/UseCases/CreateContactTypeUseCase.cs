namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ContactType.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ContactType.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ContactType.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ContactType.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreateContactTypeUseCase
{
    private readonly IContactTypeRepository _repository;
    private readonly IUnitOfWork            _unitOfWork;

    public CreateContactTypeUseCase(IContactTypeRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ContactTypeAggregate> ExecuteAsync(
        string            name,
        CancellationToken cancellationToken = default)
    {
        var contactType = new ContactTypeAggregate(new ContactTypeId(0), name);
        await _repository.AddAsync(contactType, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return contactType;
    }
}
