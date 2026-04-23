namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.BoardingPass.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.BoardingPass.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.BoardingPass.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.BoardingPass.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreateBoardingPassUseCase
{
    private readonly IBoardingPassRepository _repository;
    private readonly IUnitOfWork             _unitOfWork;

    public CreateBoardingPassUseCase(IBoardingPassRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<BoardingPassAggregate> ExecuteAsync(
        int               checkInId,
        int?              gateId,
        string?           boardingGroup,
        int               flightSeatId,
        CancellationToken cancellationToken = default)
    {
        // BoardingPassId(1) es placeholder; EF Core asigna el Id real al insertar.
        var boardingPass = new BoardingPassAggregate(
            new BoardingPassId(await GetNextIdAsync(cancellationToken)),
            checkInId, gateId, boardingGroup, flightSeatId);

        await _repository.AddAsync(boardingPass, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return boardingPass;
    }

    private async Task<int> GetNextIdAsync(CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return items.Select(x => x.Id.Value).DefaultIfEmpty(0).Max() + 1;
    }
}
