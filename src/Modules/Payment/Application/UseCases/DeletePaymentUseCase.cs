namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class DeletePaymentUseCase
{
    private readonly IPaymentRepository _repository;
    private readonly IUnitOfWork        _unitOfWork;

    public DeletePaymentUseCase(IPaymentRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(new PaymentId(id), cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
