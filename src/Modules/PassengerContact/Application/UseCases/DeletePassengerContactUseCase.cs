namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerContact.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerContact.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerContact.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class DeletePassengerContactUseCase
{
    private readonly IPassengerContactRepository _repository;
    private readonly IUnitOfWork                 _unitOfWork;

    public DeletePassengerContactUseCase(
        IPassengerContactRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(new PassengerContactId(id), cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
