namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

/// <summary>
/// Actualiza el precio de una combinación (vuelo + clase + tarifa).
/// scheduled_flight_id, cabin_class_id y fare_type_id son inmutables.
/// </summary>
public sealed class UpdateFlightCabinPriceUseCase
{
    private readonly IFlightCabinPriceRepository _repository;
    private readonly IUnitOfWork                 _unitOfWork;

    public UpdateFlightCabinPriceUseCase(
        IFlightCabinPriceRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int id, decimal newPrice, CancellationToken cancellationToken = default)
    {
        var fcp = await _repository.GetByIdAsync(new FlightCabinPriceId(id), cancellationToken)
            ?? throw new KeyNotFoundException($"FlightCabinPrice with id {id} was not found.");

        fcp.UpdatePrice(newPrice);
        await _repository.UpdateAsync(fcp, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
