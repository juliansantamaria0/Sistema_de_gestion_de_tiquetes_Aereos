namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreateSeatMapUseCase
{
    private readonly ISeatMapRepository _repository;
    private readonly IUnitOfWork        _unitOfWork;

    public CreateSeatMapUseCase(ISeatMapRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<SeatMapAggregate> ExecuteAsync(
        int               aircraftTypeId,
        string            seatNumber,
        int               cabinClassId,
        string?           seatFeatures,
        CancellationToken cancellationToken = default)
    {
        // SeatMapId(1) es placeholder; EF Core asigna el Id real al insertar.
        var seatMap = new SeatMapAggregate(
            new SeatMapId(1),
            aircraftTypeId,
            seatNumber,
            cabinClassId,
            seatFeatures);

        await _repository.AddAsync(seatMap, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return seatMap;
    }
}
