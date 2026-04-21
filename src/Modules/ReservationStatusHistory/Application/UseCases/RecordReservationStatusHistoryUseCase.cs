namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class RecordReservationStatusHistoryUseCase
{
    private readonly IReservationStatusHistoryRepository _repository;
    private readonly IUnitOfWork                         _unitOfWork;

    public RecordReservationStatusHistoryUseCase(
        IReservationStatusHistoryRepository repository,
        IUnitOfWork                         unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ReservationStatusHistoryAggregate> ExecuteAsync(
        int               reservationId,
        int               reservationStatusId,
        string?           notes,
        CancellationToken cancellationToken = default)
    {
        var entry = new ReservationStatusHistoryAggregate(
            new ReservationStatusHistoryId(1),
            reservationId, reservationStatusId,
            DateTime.UtcNow, notes);

        await _repository.AddAsync(entry, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return entry;
    }
}
