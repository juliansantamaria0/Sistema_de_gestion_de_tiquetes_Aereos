namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

/// <summary>
/// Cambia el estado de un asiento en un vuelo concreto.
/// Es la única mutación válida sobre flight_seat tras su creación.
/// Ejemplos: AVAILABLE → OCCUPIED (reserva), OCCUPIED → AVAILABLE (cancelación).
/// El trigger RF-6 en la BD verifica que el asiento esté AVAILABLE antes
/// de insertar en reservation_detail.
/// </summary>
public sealed class ChangeFlightSeatStatusUseCase
{
    private readonly IFlightSeatRepository _repository;
    private readonly IUnitOfWork           _unitOfWork;

    public ChangeFlightSeatStatusUseCase(IFlightSeatRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int               id,
        int               seatStatusId,
        CancellationToken cancellationToken = default)
    {
        var flightSeat = await _repository.GetByIdAsync(new FlightSeatId(id), cancellationToken)
            ?? throw new KeyNotFoundException($"FlightSeat with id {id} was not found.");

        flightSeat.ChangeStatus(seatStatusId);
        await _repository.UpdateAsync(flightSeat, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
