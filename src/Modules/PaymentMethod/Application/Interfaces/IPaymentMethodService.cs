namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentMethod.Application.Interfaces;

public interface IPaymentMethodService
{
    Task<PaymentMethodDto?>             GetByIdAsync(int id,            CancellationToken cancellationToken = default);
    Task<IEnumerable<PaymentMethodDto>> GetAllAsync(                    CancellationToken cancellationToken = default);
    Task<PaymentMethodDto>              CreateAsync(string name,        CancellationToken cancellationToken = default);
    Task                                UpdateAsync(int id, string name,CancellationToken cancellationToken = default);
    Task                                DeleteAsync(int id,             CancellationToken cancellationToken = default);
}

public sealed record PaymentMethodDto(int Id, string Name);
