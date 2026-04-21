namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentStatus.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentStatus.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentStatus.Application.UseCases;

public sealed class PaymentStatusService : IPaymentStatusService
{
    private readonly CreatePaymentStatusUseCase   _create;
    private readonly DeletePaymentStatusUseCase   _delete;
    private readonly GetAllPaymentStatusesUseCase _getAll;
    private readonly GetPaymentStatusByIdUseCase  _getById;
    private readonly UpdatePaymentStatusUseCase   _update;

    public PaymentStatusService(
        CreatePaymentStatusUseCase   create,
        DeletePaymentStatusUseCase   delete,
        GetAllPaymentStatusesUseCase getAll,
        GetPaymentStatusByIdUseCase  getById,
        UpdatePaymentStatusUseCase   update)
    {
        _create  = create;
        _delete  = delete;
        _getAll  = getAll;
        _getById = getById;
        _update  = update;
    }

    public async Task<PaymentStatusDto> CreateAsync(
        string            name,
        CancellationToken cancellationToken = default)
    {
        var agg = await _create.ExecuteAsync(name, cancellationToken);
        return new PaymentStatusDto(agg.Id.Value, agg.Name);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await _delete.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<PaymentStatusDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var list = await _getAll.ExecuteAsync(cancellationToken);
        return list.Select(a => new PaymentStatusDto(a.Id.Value, a.Name));
    }

    public async Task<PaymentStatusDto?> GetByIdAsync(
        int               id,
        CancellationToken cancellationToken = default)
    {
        var agg = await _getById.ExecuteAsync(id, cancellationToken);
        return agg is null ? null : new PaymentStatusDto(agg.Id.Value, agg.Name);
    }

    public async Task UpdateAsync(
        int               id,
        string            name,
        CancellationToken cancellationToken = default)
        => await _update.ExecuteAsync(id, name, cancellationToken);
}
