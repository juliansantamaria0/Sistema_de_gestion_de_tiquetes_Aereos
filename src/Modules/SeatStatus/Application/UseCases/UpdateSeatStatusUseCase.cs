namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatStatus.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatStatus.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatStatus.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class UpdateSeatStatusUseCase
{
    private readonly ISeatStatusRepository _repository;
    private readonly IUnitOfWork           _unitOfWork;

    public UpdateSeatStatusUseCase(ISeatStatusRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int               id,
        string            newName,
        CancellationToken cancellationToken = default)
    {
        var seatStatus = await _repository.GetByIdAsync(new SeatStatusId(id), cancellationToken)
            ?? throw new KeyNotFoundException($"SeatStatus with id {id} was not found.");

        seatStatus.UpdateName(newName);
        await _repository.UpdateAsync(seatStatus, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
