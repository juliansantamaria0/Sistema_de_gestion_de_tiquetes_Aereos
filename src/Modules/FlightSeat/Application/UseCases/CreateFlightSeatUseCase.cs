namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreateFlightSeatUseCase
{
    private readonly IFlightSeatRepository _repository;
    private readonly IUnitOfWork           _unitOfWork;

    public CreateFlightSeatUseCase(IFlightSeatRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<FlightSeatAggregate> ExecuteAsync(
        int               scheduledFlightId,
        int               seatMapId,
        int               seatStatusId,
        CancellationToken cancellationToken = default)
    {
        // FlightSeatId(1) es placeholder; EF Core asigna el Id real al insertar.
        var flightSeat = new FlightSeatAggregate(
            new FlightSeatId(1),
            scheduledFlightId,
            seatMapId,
            seatStatusId,
            DateTime.UtcNow);

        await _repository.AddAsync(flightSeat, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return flightSeat;
    }
}
