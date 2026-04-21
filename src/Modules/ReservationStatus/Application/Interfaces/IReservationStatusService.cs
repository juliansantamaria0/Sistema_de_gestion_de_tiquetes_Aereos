namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatus.Application.Interfaces;

public interface IReservationStatusService
{
    Task<ReservationStatusDto?>             GetByIdAsync(int id,           CancellationToken cancellationToken = default);
    Task<IEnumerable<ReservationStatusDto>> GetAllAsync(                   CancellationToken cancellationToken = default);
    Task<ReservationStatusDto>              CreateAsync(string name,       CancellationToken cancellationToken = default);
    Task                                    UpdateAsync(int id, string name,CancellationToken cancellationToken = default);
    Task                                    DeleteAsync(int id,            CancellationToken cancellationToken = default);
}

public sealed record ReservationStatusDto(int Id, string Name);
