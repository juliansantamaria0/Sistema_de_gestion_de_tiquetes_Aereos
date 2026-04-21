namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreatePaymentUseCase
{
    private readonly IPaymentRepository _repository;
    private readonly IUnitOfWork        _unitOfWork;

    public CreatePaymentUseCase(IPaymentRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PaymentAggregate> ExecuteAsync(
        CreatePaymentRequest request,
        CancellationToken    cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        // PaymentId(1) es placeholder; EF Core asigna el Id real al insertar.
        var payment = new PaymentAggregate(
            new PaymentId(1),
            request.ReservationId,
            request.TicketId,
            request.CurrencyId,
            now,
            request.Amount,
            request.PaymentStatusId,
            request.PaymentMethodId,
            request.TransactionReference,
            request.RejectionReason,
            now);

        await _repository.AddAsync(payment, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return payment;
    }
}
