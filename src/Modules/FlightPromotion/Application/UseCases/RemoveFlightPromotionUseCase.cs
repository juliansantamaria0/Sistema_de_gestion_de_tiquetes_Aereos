namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightPromotion.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightPromotion.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightPromotion.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

/// <summary>Elimina la asignación de una promoción a un vuelo.</summary>
public sealed class RemoveFlightPromotionUseCase
{
    private readonly IFlightPromotionRepository _repository;
    private readonly IUnitOfWork                _unitOfWork;

    public RemoveFlightPromotionUseCase(
        IFlightPromotionRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(new FlightPromotionId(id), cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
