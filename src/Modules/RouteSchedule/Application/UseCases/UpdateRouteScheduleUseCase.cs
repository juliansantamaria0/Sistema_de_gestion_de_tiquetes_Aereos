namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class UpdateRouteScheduleUseCase
{
    private readonly IRouteScheduleRepository _repository;
    private readonly IUnitOfWork              _unitOfWork;

    public UpdateRouteScheduleUseCase(IRouteScheduleRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int               id,
        byte              dayOfWeek,
        TimeOnly          departureTime,
        CancellationToken cancellationToken = default)
    {
        var routeSchedule = await _repository.GetByIdAsync(new RouteScheduleId(id), cancellationToken)
            ?? throw new KeyNotFoundException($"RouteSchedule with id {id} was not found.");

        routeSchedule.Update(dayOfWeek, departureTime);
        await _repository.UpdateAsync(routeSchedule, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
