namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class UpdateLoyaltyProgramUseCase
{
    private readonly ILoyaltyProgramRepository _repository;
    private readonly IUnitOfWork               _unitOfWork;

    public UpdateLoyaltyProgramUseCase(ILoyaltyProgramRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int               id,
        string            name,
        decimal           milesPerDollar,
        CancellationToken cancellationToken = default)
    {
        var program = await _repository.GetByIdAsync(new LoyaltyProgramId(id), cancellationToken)
            ?? throw new KeyNotFoundException($"LoyaltyProgram with id {id} was not found.");

        program.Update(name, milesPerDollar);
        await _repository.UpdateAsync(program, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
