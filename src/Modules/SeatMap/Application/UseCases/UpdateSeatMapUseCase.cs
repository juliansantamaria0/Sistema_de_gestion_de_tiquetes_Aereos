namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

/// <summary>
/// Actualiza la clase de cabina y/o las características de un asiento.
/// AircraftTypeId y SeatNumber son inmutables (forman la clave de negocio).
/// </summary>
public sealed class UpdateSeatMapUseCase
{
    private readonly ISeatMapRepository _repository;
    private readonly IUnitOfWork        _unitOfWork;

    public UpdateSeatMapUseCase(ISeatMapRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int               id,
        int               cabinClassId,
        string?           seatFeatures,
        CancellationToken cancellationToken = default)
    {
        var seatMap = await _repository.GetByIdAsync(new SeatMapId(id), cancellationToken)
            ?? throw new KeyNotFoundException($"SeatMap with id {id} was not found.");

        seatMap.Update(cabinClassId, seatFeatures);
        await _repository.UpdateAsync(seatMap, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
