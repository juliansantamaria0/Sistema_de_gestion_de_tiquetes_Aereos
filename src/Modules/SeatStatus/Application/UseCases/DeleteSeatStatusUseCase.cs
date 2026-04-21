namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatStatus.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatStatus.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatStatus.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class DeleteSeatStatusUseCase
{
    private readonly ISeatStatusRepository _repository;
    private readonly IUnitOfWork           _unitOfWork;

    public DeleteSeatStatusUseCase(ISeatStatusRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(new SeatStatusId(id), cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
