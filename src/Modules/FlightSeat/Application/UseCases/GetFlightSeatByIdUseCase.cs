namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Domain.ValueObject;

public sealed class GetFlightSeatByIdUseCase
{
    private readonly IFlightSeatRepository _repository;

    public GetFlightSeatByIdUseCase(IFlightSeatRepository repository)
    {
        _repository = repository;
    }

    public async Task<FlightSeatAggregate?> ExecuteAsync(
        int               id,
        CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(new FlightSeatId(id), cancellationToken);
}
