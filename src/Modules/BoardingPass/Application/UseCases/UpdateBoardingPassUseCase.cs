namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.BoardingPass.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.BoardingPass.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.BoardingPass.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;






public sealed class UpdateBoardingPassUseCase
{
    private readonly IBoardingPassRepository _repository;
    private readonly IUnitOfWork             _unitOfWork;

    public UpdateBoardingPassUseCase(IBoardingPassRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int               id,
        int?              gateId,
        string?           boardingGroup,
        CancellationToken cancellationToken = default)
    {
        var boardingPass = await _repository.GetByIdAsync(new BoardingPassId(id), cancellationToken)
            ?? throw new KeyNotFoundException($"BoardingPass with id {id} was not found.");

        boardingPass.Update(gateId, boardingGroup);
        await _repository.UpdateAsync(boardingPass, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
