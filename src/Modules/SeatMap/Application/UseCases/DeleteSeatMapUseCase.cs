namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class DeleteSeatMapUseCase
{
    private readonly ISeatMapRepository _repository;
    private readonly IUnitOfWork        _unitOfWork;

    public DeleteSeatMapUseCase(ISeatMapRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(new SeatMapId(id), cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
