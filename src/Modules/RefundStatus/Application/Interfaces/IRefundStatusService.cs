namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.RefundStatus.Application.Interfaces;

public interface IRefundStatusService
{
    Task<RefundStatusDto?>             GetByIdAsync(int id,            CancellationToken cancellationToken = default);
    Task<IEnumerable<RefundStatusDto>> GetAllAsync(                    CancellationToken cancellationToken = default);
    Task<RefundStatusDto>              CreateAsync(string name,        CancellationToken cancellationToken = default);
    Task                               UpdateAsync(int id, string name,CancellationToken cancellationToken = default);
    Task                               DeleteAsync(int id,             CancellationToken cancellationToken = default);
}

public sealed record RefundStatusDto(int Id, string Name);
