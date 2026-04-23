namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.BaggageAllowance.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.BaggageAllowance.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.BaggageAllowance.Domain.Repositories;





public sealed class GetBaggageAllowanceByCabinAndFareUseCase
{
    private readonly IBaggageAllowanceRepository _repository;

    public GetBaggageAllowanceByCabinAndFareUseCase(IBaggageAllowanceRepository repository)
    {
        _repository = repository;
    }

    public async Task<BaggageAllowanceAggregate?> ExecuteAsync(
        int               cabinClassId,
        int               fareTypeId,
        CancellationToken cancellationToken = default)
        => await _repository.GetByCabinAndFareAsync(cabinClassId, fareTypeId, cancellationToken);
}
