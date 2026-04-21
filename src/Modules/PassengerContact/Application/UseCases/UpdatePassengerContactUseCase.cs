namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerContact.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerContact.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerContact.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class UpdatePassengerContactUseCase
{
    private readonly IPassengerContactRepository _repository;
    private readonly IUnitOfWork                 _unitOfWork;

    public UpdatePassengerContactUseCase(
        IPassengerContactRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int               id,
        string            fullName,
        string            phone,
        string?           relationship,
        CancellationToken cancellationToken = default)
    {
        var contact = await _repository.GetByIdAsync(new PassengerContactId(id), cancellationToken)
            ?? throw new KeyNotFoundException($"PassengerContact with id {id} was not found.");

        contact.Update(fullName, phone, relationship);
        await _repository.UpdateAsync(contact, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
