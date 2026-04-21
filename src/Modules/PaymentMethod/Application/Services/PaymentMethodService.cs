namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentMethod.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentMethod.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentMethod.Application.UseCases;

public sealed class PaymentMethodService : IPaymentMethodService
{
    private readonly CreatePaymentMethodUseCase   _create;
    private readonly DeletePaymentMethodUseCase   _delete;
    private readonly GetAllPaymentMethodsUseCase  _getAll;
    private readonly GetPaymentMethodByIdUseCase  _getById;
    private readonly UpdatePaymentMethodUseCase   _update;

    public PaymentMethodService(
        CreatePaymentMethodUseCase  create,
        DeletePaymentMethodUseCase  delete,
        GetAllPaymentMethodsUseCase getAll,
        GetPaymentMethodByIdUseCase getById,
        UpdatePaymentMethodUseCase  update)
    {
        _create  = create;
        _delete  = delete;
        _getAll  = getAll;
        _getById = getById;
        _update  = update;
    }

    public async Task<PaymentMethodDto> CreateAsync(
        string            name,
        CancellationToken cancellationToken = default)
    {
        var agg = await _create.ExecuteAsync(name, cancellationToken);
        return new PaymentMethodDto(agg.Id.Value, agg.Name);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await _delete.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<PaymentMethodDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var list = await _getAll.ExecuteAsync(cancellationToken);
        return list.Select(a => new PaymentMethodDto(a.Id.Value, a.Name));
    }

    public async Task<PaymentMethodDto?> GetByIdAsync(
        int               id,
        CancellationToken cancellationToken = default)
    {
        var agg = await _getById.ExecuteAsync(id, cancellationToken);
        return agg is null ? null : new PaymentMethodDto(agg.Id.Value, agg.Name);
    }

    public async Task UpdateAsync(
        int               id,
        string            name,
        CancellationToken cancellationToken = default)
        => await _update.ExecuteAsync(id, name, cancellationToken);
}
