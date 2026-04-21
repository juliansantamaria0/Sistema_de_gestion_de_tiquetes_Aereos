namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class DeleteRouteScheduleUseCase
{
    private readonly IRouteScheduleRepository _repository;
    private readonly IUnitOfWork              _unitOfWork;

    public DeleteRouteScheduleUseCase(IRouteScheduleRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(new RouteScheduleId(id), cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
