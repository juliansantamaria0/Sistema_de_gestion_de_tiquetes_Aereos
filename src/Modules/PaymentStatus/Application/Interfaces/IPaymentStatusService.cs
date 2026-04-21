namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentStatus.Application.Interfaces;

public interface IPaymentStatusService
{
    Task<PaymentStatusDto?>             GetByIdAsync(int id,           CancellationToken cancellationToken = default);
    Task<IEnumerable<PaymentStatusDto>> GetAllAsync(                   CancellationToken cancellationToken = default);
    Task<PaymentStatusDto>              CreateAsync(string name,       CancellationToken cancellationToken = default);
    Task                                UpdateAsync(int id, string name,CancellationToken cancellationToken = default);
    Task                                DeleteAsync(int id,            CancellationToken cancellationToken = default);
}

public sealed record PaymentStatusDto(int Id, string Name);
