namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatus.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatus.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatus.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatus.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreateReservationStatusUseCase
{
    private readonly IReservationStatusRepository _repository;
    private readonly IUnitOfWork                  _unitOfWork;

    public CreateReservationStatusUseCase(IReservationStatusRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ReservationStatusAggregate> ExecuteAsync(
        string            name,
        CancellationToken cancellationToken = default)
    {
        // ReservationStatusId(1) es placeholder; EF Core asigna el Id real al insertar.
        var reservationStatus = new ReservationStatusAggregate(new ReservationStatusId(await GetNextIdAsync(cancellationToken)), name);

        await _repository.AddAsync(reservationStatus, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return reservationStatus;
    }

    private async Task<int> GetNextIdAsync(CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return items.Select(x => x.Id.Value).DefaultIfEmpty(0).Max() + 1;
    }
}
