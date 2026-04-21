namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreateLoyaltyProgramUseCase
{
    private readonly ILoyaltyProgramRepository _repository;
    private readonly IUnitOfWork               _unitOfWork;

    public CreateLoyaltyProgramUseCase(ILoyaltyProgramRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<LoyaltyProgramAggregate> ExecuteAsync(
        int               airlineId,
        string            name,
        decimal           milesPerDollar,
        CancellationToken cancellationToken = default)
    {
        // LoyaltyProgramId(1) es placeholder; EF Core asigna el Id real al insertar.
        var program = new LoyaltyProgramAggregate(
            new LoyaltyProgramId(1), airlineId, name, milesPerDollar);

        await _repository.AddAsync(program, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return program;
    }
}
