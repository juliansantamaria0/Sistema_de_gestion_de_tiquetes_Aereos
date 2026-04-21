namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Refund.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Refund.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Refund.Domain.Repositories;

public sealed class GetAllRefundsUseCase
{
    private readonly IRefundRepository _repository;

    public GetAllRefundsUseCase(IRefundRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<RefundAggregate>> ExecuteAsync(
        CancellationToken cancellationToken = default)
        => await _repository.GetAllAsync(cancellationToken);
}
