namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Domain.Repositories;

/// <summary>
/// Obtiene el programa de fidelización de una aerolínea.
/// La UNIQUE sobre airline_id garantiza como máximo un resultado.
/// </summary>
public sealed class GetLoyaltyProgramByAirlineUseCase
{
    private readonly ILoyaltyProgramRepository _repository;

    public GetLoyaltyProgramByAirlineUseCase(ILoyaltyProgramRepository repository)
    {
        _repository = repository;
    }

    public async Task<LoyaltyProgramAggregate?> ExecuteAsync(
        int               airlineId,
        CancellationToken cancellationToken = default)
        => await _repository.GetByAirlineAsync(airlineId, cancellationToken);
}
