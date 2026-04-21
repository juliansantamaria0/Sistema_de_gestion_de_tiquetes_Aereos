namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class DeletePassengerDiscountUseCase
{
    private readonly IPassengerDiscountRepository _repository;
    private readonly IUnitOfWork                  _unitOfWork;

    public DeletePassengerDiscountUseCase(IPassengerDiscountRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(new PassengerDiscountId(id), cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
