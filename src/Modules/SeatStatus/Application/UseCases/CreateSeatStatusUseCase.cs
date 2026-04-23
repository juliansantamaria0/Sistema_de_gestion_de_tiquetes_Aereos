namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatStatus.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatStatus.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatStatus.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatStatus.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreateSeatStatusUseCase
{
    private readonly ISeatStatusRepository _repository;
    private readonly IUnitOfWork           _unitOfWork;

    public CreateSeatStatusUseCase(ISeatStatusRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<SeatStatusAggregate> ExecuteAsync(
        string            name,
        CancellationToken cancellationToken = default)
    {
        // SeatStatusId(1) es placeholder; EF Core asigna el Id real al insertar.
        var seatStatus = new SeatStatusAggregate(new SeatStatusId(await GetNextIdAsync(cancellationToken)), name);

        await _repository.AddAsync(seatStatus, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return seatStatus;
    }

    private async Task<int> GetNextIdAsync(CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return items.Select(x => x.Id.Value).DefaultIfEmpty(0).Max() + 1;
    }
}
