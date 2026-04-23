namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreateFlightCabinPriceUseCase
{
    private readonly IFlightCabinPriceRepository _repository;
    private readonly IUnitOfWork                 _unitOfWork;

    public CreateFlightCabinPriceUseCase(
        IFlightCabinPriceRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<FlightCabinPriceAggregate> ExecuteAsync(
        int scheduledFlightId, int cabinClassId, int fareTypeId, decimal price,
        CancellationToken cancellationToken = default)
    {
        var fcp = new FlightCabinPriceAggregate(
            new FlightCabinPriceId(0),
            scheduledFlightId, cabinClassId, fareTypeId, price);

        await _repository.AddAsync(fcp, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return fcp;
    }
}
