namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Domain.Repositories;

public sealed class GetAllLoyaltyProgramsUseCase
{
    private readonly ILoyaltyProgramRepository _repository;

    public GetAllLoyaltyProgramsUseCase(ILoyaltyProgramRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<LoyaltyProgramAggregate>> ExecuteAsync(
        CancellationToken cancellationToken = default)
        => await _repository.GetAllAsync(cancellationToken);
}
