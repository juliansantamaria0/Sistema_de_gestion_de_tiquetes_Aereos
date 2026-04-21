namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ContactType.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ContactType.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ContactType.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class UpdateContactTypeUseCase
{
    private readonly IContactTypeRepository _repository;
    private readonly IUnitOfWork            _unitOfWork;

    public UpdateContactTypeUseCase(IContactTypeRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int               id,
        string            newName,
        CancellationToken cancellationToken = default)
    {
        var contactType = await _repository.GetByIdAsync(new ContactTypeId(id), cancellationToken)
            ?? throw new KeyNotFoundException($"ContactType with id {id} was not found.");

        contactType.UpdateName(newName);
        await _repository.UpdateAsync(contactType, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
