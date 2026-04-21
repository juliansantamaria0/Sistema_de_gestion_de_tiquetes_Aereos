namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Domain.ValueObject;

public sealed class GetLoyaltyProgramByIdUseCase
{
    private readonly ILoyaltyProgramRepository _repository;

    public GetLoyaltyProgramByIdUseCase(ILoyaltyProgramRepository repository)
    {
        _repository = repository;
    }

    public async Task<LoyaltyProgramAggregate?> ExecuteAsync(
        int               id,
        CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(new LoyaltyProgramId(id), cancellationToken);
}
