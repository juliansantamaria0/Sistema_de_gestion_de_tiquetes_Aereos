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
        
        var seatStatus = new SeatStatusAggregate(new SeatStatusId(0), name);

        await _repository.AddAsync(seatStatus, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return seatStatus;
    }
}
