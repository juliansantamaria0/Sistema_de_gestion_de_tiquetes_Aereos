namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.RefundStatus.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RefundStatus.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RefundStatus.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RefundStatus.Domain.ValueObject;

public sealed class GetRefundStatusByIdUseCase
{
    private readonly IRefundStatusRepository _repository;

    public GetRefundStatusByIdUseCase(IRefundStatusRepository repository)
    {
        _repository = repository;
    }

    public async Task<RefundStatusAggregate?> ExecuteAsync(
        int               id,
        CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(new RefundStatusId(id), cancellationToken);
}
