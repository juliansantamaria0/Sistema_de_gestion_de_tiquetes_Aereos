namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightPromotion.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightPromotion.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightPromotion.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightPromotion.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

/// <summary>Asigna una promoción a un vuelo programado.</summary>
public sealed class AssignFlightPromotionUseCase
{
    private readonly IFlightPromotionRepository _repository;
    private readonly IUnitOfWork                _unitOfWork;

    public AssignFlightPromotionUseCase(
        IFlightPromotionRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<FlightPromotionAggregate> ExecuteAsync(
        int scheduledFlightId, int promotionId,
        CancellationToken cancellationToken = default)
    {
        var fp = new FlightPromotionAggregate(
            new FlightPromotionId(await GetNextIdAsync(cancellationToken)), scheduledFlightId, promotionId);

        await _repository.AddAsync(fp, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return fp;
    }

    private async Task<int> GetNextIdAsync(CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return items.Select(x => x.Id.Value).DefaultIfEmpty(0).Max() + 1;
    }
}
