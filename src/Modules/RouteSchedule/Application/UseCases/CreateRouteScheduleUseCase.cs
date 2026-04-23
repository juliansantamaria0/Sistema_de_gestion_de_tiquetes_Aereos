namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreateRouteScheduleUseCase
{
    private readonly IRouteScheduleRepository _repository;
    private readonly IUnitOfWork              _unitOfWork;

    public CreateRouteScheduleUseCase(IRouteScheduleRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<RouteScheduleAggregate> ExecuteAsync(
        int             baseFlightId,
        byte            dayOfWeek,
        TimeOnly        departureTime,
        CancellationToken cancellationToken = default)
    {
        
        var routeSchedule = new RouteScheduleAggregate(
            new RouteScheduleId(0),
            baseFlightId,
            dayOfWeek,
            departureTime);

        await _repository.AddAsync(routeSchedule, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return routeSchedule;
    }
}
