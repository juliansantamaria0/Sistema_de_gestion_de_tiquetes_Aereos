namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Refund.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Refund.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Refund.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Refund.Domain.ValueObject;

public sealed class GetRefundByIdUseCase
{
    private readonly IRefundRepository _repository;

    public GetRefundByIdUseCase(IRefundRepository repository)
    {
        _repository = repository;
    }

    public async Task<RefundAggregate?> ExecuteAsync(
        int               id,
        CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(new RefundId(id), cancellationToken);
}
