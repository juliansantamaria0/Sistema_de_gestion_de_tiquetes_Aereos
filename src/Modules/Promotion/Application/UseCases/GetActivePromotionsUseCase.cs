namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Domain.Repositories;

/// <summary>
/// Obtiene todas las promociones vigentes en una fecha de referencia.
/// Una promoción está activa si: valid_from &lt;= referenceDate &lt;= valid_until.
/// Caso de uso clave para aplicar descuentos al buscar vuelos.
/// </summary>
public sealed class GetActivePromotionsUseCase
{
    private readonly IPromotionRepository _repository;

    public GetActivePromotionsUseCase(IPromotionRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<PromotionAggregate>> ExecuteAsync(
        DateOnly referenceDate, CancellationToken cancellationToken = default)
        => await _repository.GetActiveAsync(referenceDate, cancellationToken);
}
