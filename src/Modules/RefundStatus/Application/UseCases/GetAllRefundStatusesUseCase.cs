namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.RefundStatus.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RefundStatus.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RefundStatus.Domain.Repositories;

public sealed class GetAllRefundStatusesUseCase
{
    private readonly IRefundStatusRepository _repository;

    public GetAllRefundStatusesUseCase(IRefundStatusRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<RefundStatusAggregate>> ExecuteAsync(
        CancellationToken cancellationToken = default)
        => await _repository.GetAllAsync(cancellationToken);
}
