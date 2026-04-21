namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.RefundStatus.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RefundStatus.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RefundStatus.Application.UseCases;

public sealed class RefundStatusService : IRefundStatusService
{
    private readonly CreateRefundStatusUseCase   _create;
    private readonly DeleteRefundStatusUseCase   _delete;
    private readonly GetAllRefundStatusesUseCase _getAll;
    private readonly GetRefundStatusByIdUseCase  _getById;
    private readonly UpdateRefundStatusUseCase   _update;

    public RefundStatusService(
        CreateRefundStatusUseCase  create,
        DeleteRefundStatusUseCase  delete,
        GetAllRefundStatusesUseCase getAll,
        GetRefundStatusByIdUseCase getById,
        UpdateRefundStatusUseCase  update)
    {
        _create  = create;
        _delete  = delete;
        _getAll  = getAll;
        _getById = getById;
        _update  = update;
    }

    public async Task<RefundStatusDto> CreateAsync(
        string            name,
        CancellationToken cancellationToken = default)
    {
        var agg = await _create.ExecuteAsync(name, cancellationToken);
        return new RefundStatusDto(agg.Id.Value, agg.Name);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await _delete.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<RefundStatusDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var list = await _getAll.ExecuteAsync(cancellationToken);
        return list.Select(a => new RefundStatusDto(a.Id.Value, a.Name));
    }

    public async Task<RefundStatusDto?> GetByIdAsync(
        int               id,
        CancellationToken cancellationToken = default)
    {
        var agg = await _getById.ExecuteAsync(id, cancellationToken);
        return agg is null ? null : new RefundStatusDto(agg.Id.Value, agg.Name);
    }

    public async Task UpdateAsync(
        int               id,
        string            name,
        CancellationToken cancellationToken = default)
        => await _update.ExecuteAsync(id, name, cancellationToken);
}
